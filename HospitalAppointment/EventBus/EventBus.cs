using Microsoft.Extensions.DependencyInjection;

namespace HospitalAppointment.EventBus;

public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBus> _logger;

    public EventBus(IServiceProvider serviceProvider, ILogger<EventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (@event is null) throw new ArgumentNullException(nameof(@event));

        // Each publish gets its own DI scope so scoped services resolve correctly.
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>().ToList();

        _logger.LogInformation(
            "📢 Publishing {EventType} ({EventId}) to {HandlerCount} handler(s)",
            typeof(TEvent).Name, @event.EventId, handlers.Count);

        // Run handlers concurrently — they are decoupled and order is not guaranteed.
        var tasks = handlers.Select(async handler =>
        {
            try
            {
                await handler.HandleAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                // One failing handler must not break the others.
                _logger.LogError(ex,
                    "Handler {HandlerType} failed for event {EventType}",
                    handler.GetType().Name, typeof(TEvent).Name);
            }
        });

        await Task.WhenAll(tasks);
    }
}
