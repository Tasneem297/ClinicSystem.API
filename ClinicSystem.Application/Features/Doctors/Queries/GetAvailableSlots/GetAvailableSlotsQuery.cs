using AutoMapper;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using MediatR;

namespace ClinicSystem.Application.Features.Doctors.Queries.GetAvailableSlots;

public record GetAvailableSlotsQuery(Guid DoctorId, DateTime Date) : IRequest<IEnumerable<TimeSlotDto>>;

public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, IEnumerable<TimeSlotDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAvailableSlotsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TimeSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken)
            ?? await _unitOfWork.Doctors.GetByUserIdAsync(request.DoctorId.ToString(), cancellationToken);

        var doctorId = doctor?.Id ?? request.DoctorId;
        var slots = await _unitOfWork.TimeSlots.GetAvailableSlotsByDoctorAndDateAsync(
            doctorId, request.Date, cancellationToken);

        return _mapper.Map<IEnumerable<TimeSlotDto>>(slots);
    }
}
