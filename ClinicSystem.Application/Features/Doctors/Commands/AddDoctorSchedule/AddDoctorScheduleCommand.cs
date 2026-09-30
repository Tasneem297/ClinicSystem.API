using AutoMapper;
using ClinicSystem.Application.Common.Exceptions;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using MediatR;

namespace ClinicSystem.Application.Features.Doctors.Commands.AddDoctorSchedule;

public record AddDoctorScheduleCommand(
    Guid DoctorId,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime) : IRequest<DoctorScheduleDto>;

public class AddDoctorScheduleCommandHandler : IRequestHandler<AddDoctorScheduleCommand, DoctorScheduleDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddDoctorScheduleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DoctorScheduleDto> Handle(AddDoctorScheduleCommand request, CancellationToken cancellationToken)
    {
        // Support finding doctor by either Doctor.Id or Doctor.UserId (Identity ID)
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken)
            ?? await _unitOfWork.Doctors.GetByUserIdAsync(request.DoctorId.ToString(), cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), request.DoctorId);

        var schedule = new DoctorSchedule
        {
            DoctorId = doctor.Id,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await _unitOfWork.DoctorSchedules.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DoctorScheduleDto>(schedule);
    }
}
