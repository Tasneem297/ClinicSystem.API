using ClinicSystem.Application.DTOs;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterPatientAsync(string email, string password, string firstName,
        string lastName, string phone, DateTime dateOfBirth, Gender gender, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> RegisterDoctorAsync(string email, string password, string firstName,
        string lastName, string phone, string specialization, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
