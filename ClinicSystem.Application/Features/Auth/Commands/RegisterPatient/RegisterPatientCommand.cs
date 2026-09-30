using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Enums;
using MediatR;

namespace ClinicSystem.Application.Features.Auth.Commands.RegisterPatient;

public record RegisterPatientCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone,
    DateTime DateOfBirth,
    Gender Gender) : IRequest<AuthResponseDto>;

public class RegisterPatientCommandHandler : IRequestHandler<RegisterPatientCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterPatientCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterPatientAsync(
            request.Email, request.Password, request.FirstName, request.LastName,
            request.Phone, request.DateOfBirth, request.Gender, cancellationToken);
    }
}
