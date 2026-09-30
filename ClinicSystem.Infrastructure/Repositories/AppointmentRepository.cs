using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using ClinicSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;

    public AppointmentRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.FindAsync([id], cancellationToken);
    }

    public async Task<Appointment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingByStatusAsync(AppointmentStatus status, DateTime before, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.Status == status
                && a.AppointmentDate.Add(a.StartTime) > now
                && a.AppointmentDate.Add(a.StartTime) <= before)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetPastPendingAppointmentsAsync(DateTime cutoff, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.Status == AppointmentStatus.Pending
                && a.AppointmentDate.Add(a.StartTime) < cutoff)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await _context.Appointments.AddAsync(appointment, cancellationToken);
    }

    public void Update(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
    }
}
