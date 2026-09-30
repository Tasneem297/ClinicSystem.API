using System.Text;
using ClinicSystem.Application.Interfaces;
using ClinicSystem.Infrastructure.BackgroundJobs;
using ClinicSystem.Infrastructure.Identity;
using ClinicSystem.Infrastructure.Persistence;
using ClinicSystem.Infrastructure.Repositories;
using ClinicSystem.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ClinicSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ───── EF Core ─────
        services.AddDbContext<ClinicDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ClinicDbContext).Assembly.FullName)));

        // ───── Identity ─────
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ClinicDbContext>()
            .AddDefaultTokenProviders();

        // ───── JWT Authentication ─────
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        // ───── Hangfire (Separate Database) ─────
        var hangfireConnStr = configuration.GetConnectionString("HangfireConnection");
        EnsureDatabaseExists(hangfireConnStr);

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                hangfireConnStr,
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        services.AddHangfireServer();

        // ───── Repositories & Services ─────
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();

        // ───── Background Jobs ─────
        services.AddScoped<AppointmentReminderJob>();
        services.AddScoped<NoShowJob>();
        services.AddScoped<DailyScheduleJob>();

        return services;
    }

    private static void EnsureDatabaseExists(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        try
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            var targetDatabase = builder.InitialCatalog;

            if (string.IsNullOrWhiteSpace(targetDatabase) ||
                targetDatabase.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder.InitialCatalog = "master";

            using var connection = new Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $@"
                IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = '{targetDatabase.Replace("'", "''")}')
                BEGIN
                    CREATE DATABASE [{targetDatabase.Replace("]", "]]")}];
                END";
            command.ExecuteNonQuery();
        }
        catch
        {
            // If master is restricted or SQL Server instance is configured differently,
            // allow Hangfire's standard startup to continue
        }
    }
}
