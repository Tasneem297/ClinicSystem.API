using ClinicSystem.Application.DTOs;

namespace ClinicSystem.Application.Interfaces;

public interface IEmailService
{
    Task SendAppointmentReminderAsync(string toEmail, string patientName, string doctorName,
        DateTime appointmentDate, TimeSpan startTime);

    Task SendDailyScheduleAsync(string toEmail, string doctorName, IEnumerable<AppointmentDto> appointments);

    Task SendAppointmentConfirmationAsync(string toEmail, string patientName,
        DateTime appointmentDate, TimeSpan startTime);

    Task SendAppointmentCancellationAsync(string toEmail, string patientName, DateTime appointmentDate);
}
