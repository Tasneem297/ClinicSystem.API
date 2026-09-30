using AutoMapper;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClinicSystem.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire job: sends daily schedule summary email to each doctor.
/// </summary>
public class DailyScheduleJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyScheduleJob> _logger;

    public DailyScheduleJob(IServiceScopeFactory scopeFactory, ILogger<DailyScheduleJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task SendDailySchedulesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        var doctors = await unitOfWork.Doctors.GetAllAsync();

        foreach (var doctor in doctors)
        {
            var todayAppointments = await unitOfWork.Appointments
                .GetByDoctorIdAndDateAsync(doctor.Id, DateTime.UtcNow.Date);

            var appointmentDtos = mapper.Map<IEnumerable<AppointmentDto>>(todayAppointments);
            var doctorName = $"{doctor.FirstName} {doctor.LastName}";

            await emailService.SendDailyScheduleAsync(doctor.Email, doctorName, appointmentDtos);

            _logger.LogInformation(
                "Sent daily schedule to Dr. {DoctorName} ({Email}) with {Count} appointments",
                doctorName, doctor.Email, appointmentDtos.Count());
        }
    }
}
