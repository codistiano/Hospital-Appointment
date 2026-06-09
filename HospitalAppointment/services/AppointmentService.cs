using HospitalAppointment.Data;
using HospitalAppointment.EventBus;
using HospitalAppointment.Events;
using HospitalAppointment.Models;

namespace HospitalAppointment.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IDataStore _store;
    private readonly IEventBus _eventBus;

    public AppointmentService(IDataStore store, IEventBus eventBus)
    {
        _store = store;
        _eventBus = eventBus;
    }

    public Task<IReadOnlyList<Appointment>> GetAllAsync()
    {
        var list = _store.Appointments.Select(a =>
        {
            a.Patient = _store.Patients.FirstOrDefault(p => p.Id == a.PatientId);
            a.Doctor  = _store.Doctors.FirstOrDefault(d => d.Id == a.DoctorId);
            return a;
        }).OrderByDescending(a => a.ScheduledFor).ToList();
        return Task.FromResult<IReadOnlyList<Appointment>>(list);
    }

    public Task<Appointment?> GetByIdAsync(int id)
    {
        var appt = _store.Appointments.FirstOrDefault(a => a.Id == id);
        if (appt is not null)
        {
            appt.Patient = _store.Patients.FirstOrDefault(p => p.Id == appt.PatientId);
            appt.Doctor  = _store.Doctors.FirstOrDefault(d => d.Id == appt.DoctorId);
        }
        return Task.FromResult(appt);
    }

    public Task<IReadOnlyList<Doctor>> GetDoctorsAsync()
        => Task.FromResult<IReadOnlyList<Doctor>>(_store.Doctors.ToList());

    public async Task<Appointment> ScheduleAsync(int patientId, int doctorId, DateTime scheduledFor, string reason)
    {
        var patient = _store.Patients.FirstOrDefault(p => p.Id == patientId)
            ?? throw new InvalidOperationException($"Patient {patientId} not found.");
        var doctor = _store.Doctors.FirstOrDefault(d => d.Id == doctorId)
            ?? throw new InvalidOperationException($"Doctor {doctorId} not found.");

        var appointment = new Appointment
        {
            Id = _store.NextAppointmentId(),
            PatientId = patientId,
            DoctorId = doctorId,
            ScheduledFor = scheduledFor,
            Reason = string.IsNullOrWhiteSpace(reason) ? "General consultation" : reason,
            Status = AppointmentStatus.Scheduled
        };

        _store.Appointments.Add(appointment);

        // 🔔 PUBLISH EVENT — handlers run automatically via DI-based Event Bus
        await _eventBus.PublishAsync(new AppointmentScheduledEvent(
            AppointmentId: appointment.Id,
            PatientId: patient.Id,
            PatientName: patient.FullName,
            PatientEmail: patient.Email,
            DoctorId: doctor.Id,
            DoctorName: doctor.FullName,
            ScheduledFor: appointment.ScheduledFor,
            Reason: appointment.Reason
        ));

        return appointment;
    }

    public async Task<bool> CheckInAsync(int appointmentId)
    {
        var appt = _store.Appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appt is null) return false;
        if (appt.Status != AppointmentStatus.Scheduled) return false;

        appt.Status = AppointmentStatus.CheckedIn;
        appt.CheckedInAt = DateTime.UtcNow;

        var patient = _store.Patients.First(p => p.Id == appt.PatientId);
        var doctor  = _store.Doctors.First(d => d.Id == appt.DoctorId);

        // 🔔 PUBLISH EVENT
        await _eventBus.PublishAsync(new PatientCheckedInEvent(
            AppointmentId: appt.Id,
            PatientId: patient.Id,
            PatientName: patient.FullName,
            DoctorId: doctor.Id,
            DoctorName: doctor.FullName,
            CheckedInAt: appt.CheckedInAt.Value
        ));

        return true;
    }
}
