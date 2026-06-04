using HospitalAppointment.Models;

namespace HospitalAppointment.Services;

public interface IAppointmentService
{
    Task<IReadOnlyList<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(int id);
    Task<IReadOnlyList<Doctor>> GetDoctorsAsync();
    Task<Appointment> ScheduleAsync(int patientId, int doctorId, DateTime scheduledFor, string reason);
    Task<bool> CheckInAsync(int appointmentId);
}
