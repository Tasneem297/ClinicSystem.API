using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Appointment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetUpcomingByStatusAsync(AppointmentStatus status, DateTime before, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetPastPendingAppointmentsAsync(DateTime cutoff, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    void Update(Appointment appointment);
}
