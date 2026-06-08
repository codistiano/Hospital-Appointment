using HospitalAppointment.Data;
using HospitalAppointment.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointment.Controllers;

public class HomeController : Controller
{
    private readonly IDataStore _store;
    public HomeController(IDataStore store) => _store = store;

    public IActionResult Index()
    {
        ViewBag.PatientCount      = _store.Patients.Count;
        ViewBag.DoctorCount       = _store.Doctors.Count;
        ViewBag.AppointmentCount  = _store.Appointments.Count;
        ViewBag.PrescriptionCount = _store.Prescriptions.Count;
        return View(_store.AuditLog.OrderByDescending(a => a.Timestamp).Take(20).ToList());
    }

    public IActionResult Error() => View();
}
