using AutoMapper;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using MediatR;

namespace ClinicSystem.Application.Features.Doctors.Queries.GetDoctorSchedule;

public record GetDoctorScheduleQuery(Guid DoctorId) : IRequest<IEnumerable<DoctorScheduleDto>>;

public class GetDoctorScheduleQueryHandler : IRequestHandler<GetDoctorScheduleQuery, IEnumerable<DoctorScheduleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDoctorScheduleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DoctorScheduleDto>> Handle(GetDoctorScheduleQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken)
            ?? await _unitOfWork.Doctors.GetByUserIdAsync(request.DoctorId.ToString(), cancellationToken);

        var doctorId = doctor?.Id ?? request.DoctorId;
        var schedules = await _unitOfWork.DoctorSchedules.GetByDoctorIdAsync(doctorId, cancellationToken);
        return _mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules);
    }
}
