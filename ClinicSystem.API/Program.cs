using ClinicSystem.Application;
using ClinicSystem.Infrastructure;
using ClinicSystem.Infrastructure.BackgroundJobs;
using ClinicSystem.API.Middleware;
using ClinicSystem.Infrastructure.Persistence;
using Hangfire;
using Microsoft.OpenApi.Models;

namespace ClinicSystem.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add layers
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Swagger with JWT support
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic System API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT token directly (e.g. eyJhbGciOi...)",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var app = builder.Build();

        // Middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic System API v1"));
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Hangfire Dashboard
        app.UseHangfireDashboard("/hangfire");

        // Register recurring Hangfire jobs
        RecurringJob.AddOrUpdate<AppointmentReminderJob>(
            "reminder-24h",
            job => job.SendRemindersAsync(24),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<AppointmentReminderJob>(
            "reminder-1h",
            job => job.SendRemindersAsync(1),
            "*/15 * * * *"); // every 15 minutes

        RecurringJob.AddOrUpdate<NoShowJob>(
            "mark-noshow",
            job => job.MarkNoShowAppointmentsAsync(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<DailyScheduleJob>(
            "daily-schedule",
            job => job.SendDailySchedulesAsync(),
            Cron.Daily(7)); // every day at 7 AM

        // Auto-create database & seed roles/admin
        await DbInitializer.InitializeAsync(app.Services);

        await app.RunAsync();
    }
}
