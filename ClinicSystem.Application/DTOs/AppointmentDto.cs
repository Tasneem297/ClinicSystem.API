namespace ClinicSystem.Application.DTOs;

public record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    string PatientName,
    string DoctorName,
    DateTime AppointmentDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string Status,
    string? Notes);
