using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClinicSystem.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire job: marks past pending appointments as NoShow.
/// </summary>
public class NoShowJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NoShowJob> _logger;

    public NoShowJob(IServiceScopeFactory scopeFactory, ILogger<NoShowJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task MarkNoShowAppointmentsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Appointments that are still Pending and their time has passed by 1 hour
        var cutoff = DateTime.UtcNow.AddHours(-1);
        var missedAppointments = await unitOfWork.Appointments.GetPastPendingAppointmentsAsync(cutoff);

        var count = 0;
        foreach (var appointment in missedAppointments)
        {
            appointment.Status = AppointmentStatus.NoShow;
            appointment.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Appointments.Update(appointment);
            count++;
        }

        if (count > 0)
        {
            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Marked {Count} appointments as NoShow", count);
        }
    }
}
