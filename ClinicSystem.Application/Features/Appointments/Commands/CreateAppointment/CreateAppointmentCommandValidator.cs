using FluentValidation;

namespace ClinicSystem.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("Patient is required.");
        RuleFor(x => x.DoctorId).NotEmpty().WithMessage("Doctor is required.");

        RuleFor(x => x.AppointmentDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Appointment date must be today or in the future.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("Start time must be before end time.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");
    }
}
