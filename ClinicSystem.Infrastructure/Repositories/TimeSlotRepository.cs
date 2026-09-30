using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly ClinicDbContext _context;

    public TimeSlotRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<TimeSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlots.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlots
            .Include(ts => ts.Doctor)
            .Where(ts => ts.DoctorId == doctorId
                && ts.Date.Date == date.Date
                && !ts.IsBooked)
            .OrderBy(ts => ts.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TimeSlot> timeSlots, CancellationToken cancellationToken = default)
    {
        await _context.TimeSlots.AddRangeAsync(timeSlots, cancellationToken);
    }

    public void Update(TimeSlot timeSlot)
    {
        _context.TimeSlots.Update(timeSlot);
    }
}
