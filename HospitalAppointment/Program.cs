using HospitalAppointment.Data;
using HospitalAppointment.EventBus;
using HospitalAppointment.Events;
using HospitalAppointment.Handlers;
using HospitalAppointment.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// In-memory data store (singleton so data persists for the app lifetime)
builder.Services.AddSingleton<IDataStore, InMemoryDataStore>();

// Event Bus (singleton — single dispatcher for the whole app)
builder.Services.AddSingleton<IEventBus, EventBus>();

// Business services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

// ─────────────────────────────────────────────────────────────
// EVENT HANDLERS — at least 2 handlers per event (decoupled consumers)
// ─────────────────────────────────────────────────────────────

// AppointmentScheduled → Email + Audit Log
builder.Services.AddTransient<IEventHandler<AppointmentScheduledEvent>, AppointmentEmailHandler>();
builder.Services.AddTransient<IEventHandler<AppointmentScheduledEvent>, AppointmentLogHandler>();

// PatientCheckedIn → Doctor Notification + Audit Log
builder.Services.AddTransient<IEventHandler<PatientCheckedInEvent>, DoctorNotificationHandler>();
builder.Services.AddTransient<IEventHandler<PatientCheckedInEvent>, CheckInLogHandler>();

// PrescriptionIssued → Pharmacy Notification + Audit Log
builder.Services.AddTransient<IEventHandler<PrescriptionIssuedEvent>, PharmacyNotificationHandler>();
builder.Services.AddTransient<IEventHandler<PrescriptionIssuedEvent>, PrescriptionLogHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed sample data
using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetRequiredService<IDataStore>();
    store.SeedSampleData();
}

app.Run();
