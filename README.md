# 🏥 Hospital Appointment System — Event-Driven Programming Project

**Course:** Event-Driven Programming (C# / .NET)
**Platform:** ASP.NET Core 8 MVC
**Pattern:** Event-Driven Architecture (EDA) with a DI-based Event Bus

---

## 👥 Group Members

1. Abedela Bedru (@codistiano)
2. Mubarek Mustafa (@mubarekM)
3. Nehemiah Zebene (@Novsta)
4. Abduselam Nurhussen (@AbduselamNur)
5. Perez Endale (@MMKSRF)
6. Daniel Dereje (@Dayele-1997)
7. Dawit Ayana (@Devaor71)

---

## 🎯 Project Goal

Implement a simple Hospital Appointment management system that demonstrates **decoupled, asynchronous, event-driven** business logic. Every important state change (a new appointment, a patient check-in, a new prescription) is published as an **event**, and **multiple independent consumers** react to it without the publisher knowing they exist.

---

## 🔔 The Three Events

| #    | Event                       | Published By                       | Consumers (≥ 2 each)                                         |
| ---- | --------------------------- | ---------------------------------- | ------------------------------------------------------------ |
| 1    | `AppointmentScheduledEvent` | `AppointmentService.ScheduleAsync` | `AppointmentEmailHandler` (sends confirmation email), `AppointmentLogHandler` (writes audit entry) |
| 2    | `PatientCheckedInEvent`     | `AppointmentService.CheckInAsync`  | `DoctorNotificationHandler` (alerts doctor's queue), `CheckInLogHandler` (writes audit entry) |
| 3    | `PrescriptionIssuedEvent`   | `PrescriptionService.IssueAsync`   | `PharmacyNotificationHandler` (sends Rx to pharmacy), `PrescriptionLogHandler` (writes audit entry) |

---

## 🧭 Event Flow Diagram

```
┌────────────────┐     publishes     ┌─────────────┐    dispatches     ┌──────────────────────────────┐
│   Controller   │ ─── event ───►    │  EventBus   │ ─── async ───►    │ All registered IEventHandler │
│ (e.g. POST     │                   │  (DI-based) │                   │  ┌────────────────────────┐  │
│  /Appointment/ │                   │             │                   │  │ EmailHandler           │  │
│  Create)       │                   │             │                   │  │ AuditLogHandler        │  │
└────────┬───────┘                   └─────────────┘                   │  │ DoctorNotification...  │  │
         │ calls                                                        │  │ PharmacyNotification.. │  │
         ▼                                                              │  └────────────────────────┘  │
┌────────────────┐                                                      │   (run concurrently,         │
│ Business       │  ── _eventBus.PublishAsync(event) ──────────────►    │    one failure doesn't stop  │
│ Service        │                                                      │    the others)               │
│ (Publisher)    │                                                      └──────────────────────────────┘
└────────────────┘
```

The publisher only knows about `IEventBus`. Adding/removing a handler is a **one-line change in `Program.cs`** — no service code is touched. This is the decoupling the assignment asks for.

---

## 🏛️ Architecture Highlights (mapping to Section II of the brief)

- **Event Publisher (Producer):** `AppointmentService`, `PrescriptionService` — call `_eventBus.PublishAsync(...)`.
- **Event Consumer (Handler):** `Handlers/EventHandlers.cs` — each implements `IEventHandler<TEvent>`. Two handlers per event.
- **Event Bus / Aggregator:** `EventBus/EventBus.cs` — uses **Dependency Injection** to discover all `IEventHandler<TEvent>` implementations and invokes them.
- **Decoupling:** Every service and the bus itself is exposed via an interface (`IAppointmentService`, `IEventBus`, `IEventHandler<>`).
- **Asynchronicity:** All handlers and the bus use `async/await`. Handlers are dispatched concurrently via `Task.WhenAll`.
- **Immutability:** Event payloads are C# `record` types — payload state cannot mutate during propagation.

---

## 📁 Project Structure

```
HospitalAppointment/
├─ Program.cs                  ← DI registration of services + handlers
├─ EventBus/
│   ├─ IEvent.cs               ← marker interface
│   ├─ IEventHandler.cs        ← generic handler contract
│   ├─ IEventBus.cs
│   └─ EventBus.cs             ← DI-based dispatcher
├─ Events/
│   ├─ AppointmentScheduledEvent.cs
│   ├─ PatientCheckedInEvent.cs
│   └─ PrescriptionIssuedEvent.cs
├─ Handlers/
│   └─ EventHandlers.cs        ← 6 handlers (2 per event)
├─ Services/
│   ├─ IAppointmentService.cs / AppointmentService.cs   (PUBLISHER)
│   ├─ IPrescriptionService.cs / PrescriptionService.cs (PUBLISHER)
│   └─ IPatientService.cs    / PatientService.cs
├─ Models/                     ← Patient, Doctor, Appointment, Prescription, AuditLogEntry
├─ Data/InMemoryDataStore.cs   ← thread-safe in-memory store + sample data
├─ Controllers/                ← HomeController, AppointmentController
└─ Views/                      ← Razor views (Bootstrap UI)
```

---

## ▶️ How to Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Any IDE: Visual Studio 2022, Rider, or VS Code with the C# Dev Kit.

### Steps

```bash
git clone <your-repo-url>
cd HospitalAppointment
dotnet restore
dotnet run --project HospitalAppointment
```

Open **https://localhost:5001** in your browser.

### Try It

1. Go to **Appointments → + New Appointment** → schedule one.
   → Watch the console: you'll see logs from `AppointmentEmailHandler` and `AppointmentLogHandler`.
2. Click **Check In** on the new appointment.
   → `DoctorNotificationHandler` and `CheckInLogHandler` fire.
3. Click **Prescribe** → fill in a medication.
   → `PharmacyNotificationHandler` and `PrescriptionLogHandler` fire.
4. Go back to **Dashboard** to see all audit log entries written by the *Log handlers.

---

## 🌿 Git Workflow Used

- `main` is protected. **No direct commits.**
- Feature branches: `feature/event-bus-setup`, `feature/appointment-scheduling`, `feature/checkin`, `feature/prescription`, `feature/views`, etc.
- Each feature merges to `main` only via **Merge Request** with at least one peer approval.

---

## ✅ Assignment Checklist

- [x] ASP.NET Core MVC project
- [x] At least 3 events (`AppointmentScheduledEvent`, `PatientCheckedInEvent`, `PrescriptionIssuedEvent`)
- [x] At least 2 different handlers per event
- [x] DI-based Event Bus that discovers `IEventHandler<TEvent>` implementations
- [x] All services exposed via interfaces (decoupling)
- [x] `async/await` throughout
- [x] Immutable event payloads (C# `record` with `init`-only properties)
- [x] README with project title, group, setup, and event flow diagram

---

## 📧 Submission Contact

Instructor: **tadegewk@gmail.com**
