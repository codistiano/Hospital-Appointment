using HospitalAppointment.EventBus;

namespace HospitalAppointment.Events;

public record PrescriptionIssuedEvent(
    int PrescriptionId,
    int AppointmentId,
    int PatientId,
    string PatientName,
    string PatientEmail,
    int DoctorId,
    string DoctorName,
    string Medication,
    string Dosage,
    string Instructions
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
