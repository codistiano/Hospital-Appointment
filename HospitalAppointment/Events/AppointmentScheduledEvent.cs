using HospitalAppointment.EventBus;

namespace HospitalAppointment.Events;


public record AppointmentScheduledEvent(
    int AppointmentId,
    int PatientId,
    string PatientName,
    string PatientEmail,
    int DoctorId,
    string DoctorName,
    DateTime ScheduledFor,
    string Reason
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
