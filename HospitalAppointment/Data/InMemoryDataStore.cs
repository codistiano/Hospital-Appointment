using HospitalAppointment.Models;

namespace HospitalAppointment.Data;

public interface IDataStore
{
    List<Patient> Patients { get; }
    List<Doctor> Doctors { get; }
    List<Appointment> Appointments { get; }
    List<Prescription> Prescriptions { get; }
    List<AuditLogEntry> AuditLog { get; }

    int NextAppointmentId();
    int NextPrescriptionId();
    int NextAuditId();
    void SeedSampleData();
}

public class InMemoryDataStore : IDataStore
{
    private readonly object _lock = new();
    private int _appointmentSeq = 0;
    private int _prescriptionSeq = 0;
    private int _auditSeq = 0;

    public List<Patient> Patients { get; } = new();
    public List<Doctor> Doctors { get; } = new();
    public List<Appointment> Appointments { get; } = new();
    public List<Prescription> Prescriptions { get; } = new();
    public List<AuditLogEntry> AuditLog { get; } = new();

    public int NextAppointmentId() { lock (_lock) { return ++_appointmentSeq; } }
    public int NextPrescriptionId() { lock (_lock) { return ++_prescriptionSeq; } }
    public int NextAuditId() { lock (_lock) { return ++_auditSeq; } }

    public void SeedSampleData()
    {
        if (Patients.Any()) return;

        Patients.AddRange(new[]
        {
            new Patient { Id = 1, FullName = "Alice Johnson", Email = "alice@example.com", Phone = "555-0101", DateOfBirth = new DateTime(1990, 5, 14) },
            new Patient { Id = 2, FullName = "Bob Smith",     Email = "bob@example.com",   Phone = "555-0102", DateOfBirth = new DateTime(1985, 8, 22) },
            new Patient { Id = 3, FullName = "Carol Davis",   Email = "carol@example.com", Phone = "555-0103", DateOfBirth = new DateTime(1978, 1, 3)  },
        });

        Doctors.AddRange(new[]
        {
            new Doctor { Id = 1, FullName = "Dr. Emily Chen",   Specialization = "Cardiology",  Email = "e.chen@hospital.com" },
            new Doctor { Id = 2, FullName = "Dr. Marcus Reyes", Specialization = "Pediatrics",  Email = "m.reyes@hospital.com" },
            new Doctor { Id = 3, FullName = "Dr. Sara Patel",   Specialization = "Dermatology", Email = "s.patel@hospital.com" },
        });
    }
}
