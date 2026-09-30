using FluentValidation;

namespace ClinicSystem.Application.Features.Auth.Commands.RegisterDoctor;

public class RegisterDoctorCommandValidator : AbstractValidator<RegisterDoctorCommand>
{
    public RegisterDoctorCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Specialization).NotEmpty().MaximumLength(200);
    }
}
