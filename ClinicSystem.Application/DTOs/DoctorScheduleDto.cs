namespace ClinicSystem.Application.DTOs;

public record DoctorScheduleDto(
    Guid Id,
    Guid DoctorId,
    string DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime);
