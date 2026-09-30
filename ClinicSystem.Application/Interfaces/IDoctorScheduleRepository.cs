using ClinicSystem.Domain.Entities;

namespace ClinicSystem.Application.Interfaces;

public interface IDoctorScheduleRepository
{
    Task<DoctorSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task AddAsync(DoctorSchedule schedule, CancellationToken cancellationToken = default);
    void Update(DoctorSchedule schedule);
    void Delete(DoctorSchedule schedule);
}
