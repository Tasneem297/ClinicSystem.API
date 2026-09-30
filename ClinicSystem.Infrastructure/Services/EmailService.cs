using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicSystem.Infrastructure.Services;

/// <summary>
/// Mock email service that logs email content. Replace with real SMTP/SendGrid in production.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAppointmentReminderAsync(string toEmail, string patientName,
        string doctorName, DateTime appointmentDate, TimeSpan startTime)
    {
        _logger.LogInformation(
            "📧 REMINDER: To={Email}, Patient={Patient}, Doctor={Doctor}, Date={Date}, Time={Time}",
            toEmail, patientName, doctorName, appointmentDate.ToShortDateString(), startTime);
        return Task.CompletedTask;
    }

    public Task SendDailyScheduleAsync(string toEmail, string doctorName,
        IEnumerable<AppointmentDto> appointments)
    {
        var count = appointments.Count();
        _logger.LogInformation(
            "📧 DAILY SCHEDULE: To={Email}, Doctor={Doctor}, AppointmentCount={Count}",
            toEmail, doctorName, count);
        return Task.CompletedTask;
    }

    public Task SendAppointmentConfirmationAsync(string toEmail, string patientName,
        DateTime appointmentDate, TimeSpan startTime)
    {
        _logger.LogInformation(
            "📧 CONFIRMATION: To={Email}, Patient={Patient}, Date={Date}, Time={Time}",
            toEmail, patientName, appointmentDate.ToShortDateString(), startTime);
        return Task.CompletedTask;
    }

    public Task SendAppointmentCancellationAsync(string toEmail, string patientName,
        DateTime appointmentDate)
    {
        _logger.LogInformation(
            "📧 CANCELLATION: To={Email}, Patient={Patient}, Date={Date}",
            toEmail, patientName, appointmentDate.ToShortDateString());
        return Task.CompletedTask;
    }
}
