using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class TimeSlot : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsBooked { get; set; }

    // Navigation
    public Doctor Doctor { get; set; } = null!;
}
