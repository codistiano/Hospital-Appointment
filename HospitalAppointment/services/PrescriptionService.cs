using HospitalAppointment.Data;
using HospitalAppointment.EventBus;
using HospitalAppointment.Events;
using HospitalAppointment.Models;

namespace HospitalAppointment.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IDataStore _store;
    private readonly IEventBus _eventBus;

    public PrescriptionService(IDataStore store, IEventBus eventBus)
    {
        _store = store;
        _eventBus = eventBus;
    }

    public Task<IReadOnlyList<Prescription>> GetAllAsync()
        => Task.FromResult<IReadOnlyList<Prescription>>(
            _store.Prescriptions.OrderByDescending(p => p.IssuedAt).ToList());

    public async Task<Prescription> IssueAsync(int appointmentId, string medication, string dosage, string instructions)
    {
        var appt = _store.Appointments.FirstOrDefault(a => a.Id == appointmentId)
            ?? throw new InvalidOperationException($"Appointment {appointmentId} not found.");
        var patient = _store.Patients.First(p => p.Id == appt.PatientId);
        var doctor  = _store.Doctors.First(d => d.Id == appt.DoctorId);

        var rx = new Prescription
        {
            Id = _store.NextPrescriptionId(),
            AppointmentId = appt.Id,
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            Medication = medication,
            Dosage = dosage,
            Instructions = instructions,
            IssuedAt = DateTime.UtcNow
        };

        _store.Prescriptions.Add(rx);

        // 🔔 PUBLISH EVENT
        await _eventBus.PublishAsync(new PrescriptionIssuedEvent(
            PrescriptionId: rx.Id,
            AppointmentId: appt.Id,
            PatientId: patient.Id,
            PatientName: patient.FullName,
            PatientEmail: patient.Email,
            DoctorId: doctor.Id,
            DoctorName: doctor.FullName,
            Medication: rx.Medication,
            Dosage: rx.Dosage,
            Instructions: rx.Instructions
        ));

        return rx;
    }
}
