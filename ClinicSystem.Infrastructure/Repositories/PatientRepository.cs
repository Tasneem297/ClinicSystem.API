using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ClinicDbContext _context;

    public PatientRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.FindAsync([id], cancellationToken);
    }

    public async Task<Patient?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Patients.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
    }

    public void Update(Patient patient)
    {
        _context.Patients.Update(patient);
    }
}
