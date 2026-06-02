using HospitalAppointment.EventBus;

namespace HospitalAppointment.Events;

public record PatientCheckedInEvent(
    int AppointmentId,
    int PatientId,
    string PatientName,
    int DoctorId,
    string DoctorName,
    DateTime CheckedInAt
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
