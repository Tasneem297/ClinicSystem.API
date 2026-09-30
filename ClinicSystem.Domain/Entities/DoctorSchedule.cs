using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class DoctorSchedule : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    // Navigation
    public Doctor Doctor { get; set; } = null!;
}
