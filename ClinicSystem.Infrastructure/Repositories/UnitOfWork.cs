using ClinicSystem.Application.Interfaces;
using ClinicSystem.Infrastructure.Persistence;

namespace ClinicSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ClinicDbContext _context;
    private IAppointmentRepository? _appointments;
    private IPatientRepository? _patients;
    private IDoctorRepository? _doctors;
    private IDoctorScheduleRepository? _doctorSchedules;
    private ITimeSlotRepository? _timeSlots;

    public UnitOfWork(ClinicDbContext context)
    {
        _context = context;
    }

    public IAppointmentRepository Appointments =>
        _appointments ??= new AppointmentRepository(_context);

    public IPatientRepository Patients =>
        _patients ??= new PatientRepository(_context);

    public IDoctorRepository Doctors =>
        _doctors ??= new DoctorRepository(_context);

    public IDoctorScheduleRepository DoctorSchedules =>
        _doctorSchedules ??= new DoctorScheduleRepository(_context);

    public ITimeSlotRepository TimeSlots =>
        _timeSlots ??= new TimeSlotRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
