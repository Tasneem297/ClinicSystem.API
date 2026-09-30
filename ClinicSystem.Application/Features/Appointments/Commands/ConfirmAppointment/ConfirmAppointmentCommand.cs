using ClinicSystem.Application.Common.Exceptions;
using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using MediatR;

namespace ClinicSystem.Application.Features.Appointments.Commands.ConfirmAppointment;

public record ConfirmAppointmentCommand(Guid AppointmentId) : IRequest<Unit>;

public class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmAppointmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        if (appointment.Status != AppointmentStatus.Pending)
            throw new BadRequestException($"Only pending appointments can be confirmed. Current status: '{appointment.Status}'.");

        appointment.Status = AppointmentStatus.Confirmed;
        appointment.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Appointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
