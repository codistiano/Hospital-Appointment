using HospitalAppointment.Data;
using HospitalAppointment.EventBus;
using HospitalAppointment.Events;
using HospitalAppointment.Models;

namespace HospitalAppointment.Handlers;

// ─── Handlers for AppointmentScheduledEvent ────────────────────────────


public class AppointmentEmailHandler : IEventHandler<AppointmentScheduledEvent>
{
    private readonly ILogger<AppointmentEmailHandler> _logger;
    public AppointmentEmailHandler(ILogger<AppointmentEmailHandler> logger) => _logger = logger;

    public async Task HandleAsync(AppointmentScheduledEvent e, CancellationToken ct = default)
    {
        await Task.Delay(50, ct); // simulate I/O
        _logger.LogInformation(
            "✉️  [EmailHandler] Sent confirmation to {Email}: appointment with {Doctor} on {When:g}",
            e.PatientEmail, e.DoctorName, e.ScheduledFor);
    }
}

public class AppointmentLogHandler : IEventHandler<AppointmentScheduledEvent>
{
    private readonly IDataStore _store;
    private readonly ILogger<AppointmentLogHandler> _logger;
    public AppointmentLogHandler(IDataStore store, ILogger<AppointmentLogHandler> logger)
    {
        _store = store;
        _logger = logger;
    }

    public Task HandleAsync(AppointmentScheduledEvent e, CancellationToken ct = default)
    {
        _store.AuditLog.Add(new AuditLogEntry
        {
            Id = _store.NextAuditId(),
            EventType = nameof(AppointmentScheduledEvent),
            Message = $"Appointment #{e.AppointmentId} scheduled for {e.PatientName} with {e.DoctorName} at {e.ScheduledFor:g}.",
            Timestamp = DateTime.UtcNow
        });
        _logger.LogInformation("📝 [LogHandler] Audit entry written for appointment #{Id}", e.AppointmentId);
        return Task.CompletedTask;
    }
}

// ─── Handlers for PatientCheckedInEvent ────────────────────────────────

public class DoctorNotificationHandler : IEventHandler<PatientCheckedInEvent>
{
    private readonly ILogger<DoctorNotificationHandler> _logger;
    public DoctorNotificationHandler(ILogger<DoctorNotificationHandler> logger) => _logger = logger;

    public async Task HandleAsync(PatientCheckedInEvent e, CancellationToken ct = default)
    {
        await Task.Delay(50, ct);
        _logger.LogInformation(
            "🔔 [DoctorNotification] {Doctor}: {Patient} has checked in for appointment #{ApptId}",
            e.DoctorName, e.PatientName, e.AppointmentId);
    }
}

public class CheckInLogHandler : IEventHandler<PatientCheckedInEvent>
{
    private readonly IDataStore _store;
    public CheckInLogHandler(IDataStore store) => _store = store;

    public Task HandleAsync(PatientCheckedInEvent e, CancellationToken ct = default)
    {
        _store.AuditLog.Add(new AuditLogEntry
        {
            Id = _store.NextAuditId(),
            EventType = nameof(PatientCheckedInEvent),
            Message = $"Patient {e.PatientName} checked in for appointment #{e.AppointmentId} at {e.CheckedInAt:g}.",
            Timestamp = DateTime.UtcNow
        });
        return Task.CompletedTask;
    }
}

// ─── Handlers for PrescriptionIssuedEvent ──────────────────────────────

public class PharmacyNotificationHandler : IEventHandler<PrescriptionIssuedEvent>
{
    private readonly ILogger<PharmacyNotificationHandler> _logger;
    public PharmacyNotificationHandler(ILogger<PharmacyNotificationHandler> logger) => _logger = logger;

    public async Task HandleAsync(PrescriptionIssuedEvent e, CancellationToken ct = default)
    {
        await Task.Delay(50, ct);
        _logger.LogInformation(
            "💊 [Pharmacy] Prescription #{Id} for {Patient}: {Med} ({Dosage})",
            e.PrescriptionId, e.PatientName, e.Medication, e.Dosage);
    }
}

public class PrescriptionLogHandler : IEventHandler<PrescriptionIssuedEvent>
{
    private readonly IDataStore _store;
    public PrescriptionLogHandler(IDataStore store) => _store = store;

    public Task HandleAsync(PrescriptionIssuedEvent e, CancellationToken ct = default)
    {
        _store.AuditLog.Add(new AuditLogEntry
        {
            Id = _store.NextAuditId(),
            EventType = nameof(PrescriptionIssuedEvent),
            Message = $"Prescription #{e.PrescriptionId} ({e.Medication}, {e.Dosage}) issued to {e.PatientName} by {e.DoctorName}.",
            Timestamp = DateTime.UtcNow
        });
        return Task.CompletedTask;
    }
}
