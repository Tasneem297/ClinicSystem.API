using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class DoctorScheduleRepository : IDoctorScheduleRepository
{
    private readonly ClinicDbContext _context;

    public DoctorScheduleRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<DoctorSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DoctorSchedules.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.DoctorSchedules
            .Where(ds => ds.DoctorId == doctorId)
            .OrderBy(ds => ds.DayOfWeek)
            .ThenBy(ds => ds.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DoctorSchedule schedule, CancellationToken cancellationToken = default)
    {
        await _context.DoctorSchedules.AddAsync(schedule, cancellationToken);
    }

    public void Update(DoctorSchedule schedule)
    {
        _context.DoctorSchedules.Update(schedule);
    }

    public void Delete(DoctorSchedule schedule)
    {
        _context.DoctorSchedules.Remove(schedule);
    }
}
