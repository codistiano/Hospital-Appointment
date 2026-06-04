using HospitalAppointment.Models;

namespace HospitalAppointment.Services;

public interface IPatientService
{
    Task<IReadOnlyList<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
}
