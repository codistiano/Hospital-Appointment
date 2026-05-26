namespace HospitalAppointment.EventBus;

public interface IEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
