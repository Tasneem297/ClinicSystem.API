namespace ClinicSystem.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAppointmentRepository Appointments { get; }
    IPatientRepository Patients { get; }
    IDoctorRepository Doctors { get; }
    IDoctorScheduleRepository DoctorSchedules { get; }
    ITimeSlotRepository TimeSlots { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
