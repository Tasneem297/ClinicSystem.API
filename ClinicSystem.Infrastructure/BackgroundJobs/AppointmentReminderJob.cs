using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClinicSystem.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire job: sends appointment reminders N hours before the appointment.
/// </summary>
public class AppointmentReminderJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppointmentReminderJob> _logger;

    public AppointmentReminderJob(IServiceScopeFactory scopeFactory, ILogger<AppointmentReminderJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task SendRemindersAsync(int hoursBeforeAppointment)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var reminderWindow = DateTime.UtcNow.AddHours(hoursBeforeAppointment);

        var appointments = await unitOfWork.Appointments
            .GetUpcomingByStatusAsync(AppointmentStatus.Confirmed, reminderWindow);

        foreach (var appointment in appointments)
        {
            var patientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}";
            var doctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}";

            await emailService.SendAppointmentReminderAsync(
                appointment.Patient.Email, patientName, doctorName,
                appointment.AppointmentDate, appointment.StartTime);

            _logger.LogInformation(
                "Sent {Hours}h reminder for appointment {Id} to {Patient}",
                hoursBeforeAppointment, appointment.Id, patientName);
        }
    }
}
