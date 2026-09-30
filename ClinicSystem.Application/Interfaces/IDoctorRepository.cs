using ClinicSystem.Domain.Entities;

namespace ClinicSystem.Application.Interfaces;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default);
    void Update(Doctor doctor);
}
