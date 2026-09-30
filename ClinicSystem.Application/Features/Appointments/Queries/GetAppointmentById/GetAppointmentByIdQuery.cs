using AutoMapper;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using MediatR;

namespace ClinicSystem.Application.Features.Appointments.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentDto?>;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAppointmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        return appointment is null ? null : _mapper.Map<AppointmentDto>(appointment);
    }
}
