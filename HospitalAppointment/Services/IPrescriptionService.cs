using HospitalAppointment.Models;

namespace HospitalAppointment.Services;

public interface IPrescriptionService
{
    Task<IReadOnlyList<Prescription>> GetAllAsync();
    Task<Prescription> IssueAsync(int appointmentId, string medication, string dosage, string instructions);
}
