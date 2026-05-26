namespace HospitalAppointment.Models;

public enum AppointmentStatus
{
    Scheduled,
    CheckedIn,
    Completed,
    Cancelled
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime ScheduledFor { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public DateTime? CheckedInAt { get; set; }

    // Convenience navigation (populated by service layer for views)
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}
