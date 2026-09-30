namespace ClinicSystem.Application.DTOs;

public record TimeSlotDto(
    Guid Id,
    Guid DoctorId,
    string DoctorName,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool IsBooked);
