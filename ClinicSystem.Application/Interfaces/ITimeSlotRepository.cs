using ClinicSystem.Domain.Entities;

namespace ClinicSystem.Application.Interfaces;

public interface ITimeSlotRepository
{
    Task<TimeSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TimeSlot> timeSlots, CancellationToken cancellationToken = default);
    void Update(TimeSlot timeSlot);
}
