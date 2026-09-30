using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;

    public DoctorRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.FindAsync([id], cancellationToken);
    }

    public async Task<Doctor?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        await _context.Doctors.AddAsync(doctor, cancellationToken);
    }

    public void Update(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
    }
}
