using FluentValidation;

namespace ClinicSystem.Application.Features.Doctors.Commands.AddDoctorSchedule;

public class AddDoctorScheduleCommandValidator : AbstractValidator<AddDoctorScheduleCommand>
{
    public AddDoctorScheduleCommandValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty().WithMessage("Doctor is required.");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("A valid day of the week is required.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("Start time must be before end time.");
    }
}
