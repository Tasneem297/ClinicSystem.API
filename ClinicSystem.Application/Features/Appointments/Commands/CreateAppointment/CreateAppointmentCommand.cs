using AutoMapper;
using ClinicSystem.Application.Common.Exceptions;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using MediatR;

namespace ClinicSystem.Application.Features.Appointments.Commands.CreateAppointment;

public record CreateAppointmentCommand(
    Guid PatientId,
    Guid DoctorId,
    DateTime AppointmentDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string? Notes) : IRequest<AppointmentDto>;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateAppointmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken)
            ?? await _unitOfWork.Patients.GetByUserIdAsync(request.PatientId.ToString(), cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken)
            ?? await _unitOfWork.Doctors.GetByUserIdAsync(request.DoctorId.ToString(), cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), request.DoctorId);

        // Check if the time slot is available
        var existingAppointments = await _unitOfWork.Appointments.GetByDoctorIdAndDateAsync(
            doctor.Id, request.AppointmentDate, cancellationToken);

        var hasConflict = existingAppointments.Any(a =>
            a.Status != AppointmentStatus.Cancelled &&
            a.Status != AppointmentStatus.NoShow &&
            a.StartTime < request.EndTime &&
            a.EndTime > request.StartTime);

        if (hasConflict)
            throw new BadRequestException("The selected time slot is not available. There is a scheduling conflict.");

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            AppointmentDate = request.AppointmentDate.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Notes = request.Notes,
            Status = AppointmentStatus.Pending
        };

        await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties for mapping
        var created = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(appointment.Id, cancellationToken);
        return _mapper.Map<AppointmentDto>(created);
    }
}
