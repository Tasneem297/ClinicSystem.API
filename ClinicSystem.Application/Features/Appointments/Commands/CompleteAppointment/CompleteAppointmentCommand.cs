using ClinicSystem.Application.Common.Exceptions;
using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using MediatR;

namespace ClinicSystem.Application.Features.Appointments.Commands.CompleteAppointment;

public record CompleteAppointmentCommand(Guid AppointmentId) : IRequest<Unit>;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompleteAppointmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new BadRequestException($"Only confirmed appointments can be completed. Current status: '{appointment.Status}'.");

        appointment.Status = AppointmentStatus.Completed;
        appointment.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Appointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
