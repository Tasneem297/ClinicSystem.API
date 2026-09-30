# 🏥 Mini Clinic Appointment System

A Clean Architecture .NET 9 Web API implementing CQRS with MediatR, Hangfire background workers with an isolated database, and ASP.NET Core Identity with JWT authentication.

---

## 🏛️ Architecture Overview

The solution follows the **Clean Architecture** pattern with 4 distinct layers:

```
ClinicSystem.API (Host, Controllers, Swagger, Hangfire UI)
       │
       ▼
ClinicSystem.Application (CQRS Commands/Queries, MediatR, FluentValidation, DTOs, Mappings)
       │
       ▼
ClinicSystem.Domain (Pure Entities, Value Objects, Enums, BaseEntity)
       ▲
       │
ClinicSystem.Infrastructure (EF Core 9, Identity, Repositories, Hangfire Jobs, Mock Email)
```

---

## 🗄️ Dual-Database Setup

To keep background processing completely decoupled from application domain data:
- **`ClinicSystemDb`**: Stores domain data (`Patients`, `Doctors`, `Appointments`, `DoctorSchedules`, `TimeSlots`) and ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`).
- **`ClinicSystemHangfireDb`**: Stores Hangfire job queues, states, execution history, servers, and locks.
- Both databases are automatically verified/created and migrated on application startup.

---

## ⚡ Core Features & CQRS Operations

### Actors & Roles
- **Patient**: Register, book appointments, cancel appointments, view appointments.
- **Doctor**: Register, manage weekly schedules, view upcoming appointments, confirm/complete appointments.
- **Admin**: Full administrative access (`admin@clinic.com` / `Admin123!`).

### State Machine Lifecycle
```
[Pending] ──(Doctor Confirms)──> [Confirmed] ──(Doctor Completes)──> [Completed]
    │                                │
    ├───────(Patient/Doctor Cancels)─┴─────────────────────────────> [Cancelled]
    │
    └───────(Hangfire auto-marks missed after 1 hour)──────────────> [NoShow]
```

### MediatR CQRS Commands & Queries
- **Commands**:
  - `RegisterPatientCommand`: Registers patient account + domain entity.
  - `RegisterDoctorCommand`: Registers doctor account with medical specialization.
  - `LoginCommand`: Validates credentials & issues 60-minute JWT token.
  - `CreateAppointmentCommand`: Validates scheduling conflicts & books in `Pending` state.
  - `ConfirmAppointmentCommand`: Doctor accepts appointment (`Pending` → `Confirmed`).
  - `CompleteAppointmentCommand`: Doctor marks appointment completed (`Confirmed` → `Completed`).
  - `CancelAppointmentCommand`: Cancels a `Pending` or `Confirmed` appointment.
  - `AddDoctorScheduleCommand`: Sets weekly working hours for a doctor.
- **Queries**:
  - `GetAppointmentByIdQuery`: Returns detailed appointment information.
  - `GetPatientAppointmentsQuery`: Returns all appointments for a patient.
  - `GetDoctorScheduleQuery`: Returns doctor's weekly working schedule.
  - `GetAvailableSlotsQuery`: Returns unbooked slots for a doctor on a given date.

---

## ⏱️ Hangfire Background Jobs

| Job ID | Recurrence | Action |
|--------|------------|--------|
| `reminder-24h` | Hourly | Scans upcoming `Confirmed` appointments in next 24h & sends reminder email. |
| `reminder-1h` | Every 15 min | Scans upcoming `Confirmed` appointments in next 1h & sends reminder email. |
| `mark-noshow` | Hourly | Automatically transitions past pending appointments to `NoShow`. |
| `daily-schedule` | Daily at 7:00 AM | Sends each doctor an email summary of today's schedule. |

---

## 🚀 Running the Project

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server LocalDB (included with Visual Studio)

### 1. Build and Run
```powershell
dotnet build ClinicSystem.API\ClinicSystem.API.sln
dotnet run --project ClinicSystem.API\ClinicSystem.API.csproj
```

### 2. Dashboards & Testing
- **Swagger UI**: `https://localhost:7232/swagger`
- **Hangfire Dashboard**: `https://localhost:7232/hangfire`
- **Default Admin Account**:
  - Email: `admin@clinic.com`
  - Password: `Admin123!`
- **HTTP Client**: Use [`ClinicSystem.API/ClinicSystem.API.http`](ClinicSystem.API/ClinicSystem.API.http) in Visual Studio to execute ready-to-test API requests.
