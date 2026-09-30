using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using MediatR;

namespace ClinicSystem.Application.Features.Auth.Commands.RegisterDoctor;

public record RegisterDoctorCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone,
    string Specialization) : IRequest<AuthResponseDto>;

public class RegisterDoctorCommandHandler : IRequestHandler<RegisterDoctorCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterDoctorCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterDoctorCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterDoctorAsync(
            request.Email, request.Password, request.FirstName, request.LastName,
            request.Phone, request.Specialization, cancellationToken);
    }
}
