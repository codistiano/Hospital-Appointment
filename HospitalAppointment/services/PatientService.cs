using HospitalAppointment.Data;
using HospitalAppointment.Models;

namespace HospitalAppointment.Services;

public class PatientService : IPatientService
{
    private readonly IDataStore _store;
    public PatientService(IDataStore store) => _store = store;

    public Task<IReadOnlyList<Patient>> GetAllAsync()
        => Task.FromResult<IReadOnlyList<Patient>>(_store.Patients.ToList());

    public Task<Patient?> GetByIdAsync(int id)
        => Task.FromResult(_store.Patients.FirstOrDefault(p => p.Id == id));
}
