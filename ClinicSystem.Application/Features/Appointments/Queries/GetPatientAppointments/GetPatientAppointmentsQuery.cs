using AutoMapper;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using MediatR;

namespace ClinicSystem.Application.Features.Appointments.Queries.GetPatientAppointments;

public record GetPatientAppointmentsQuery(Guid PatientId) : IRequest<IEnumerable<AppointmentDto>>;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPatientAppointmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken)
            ?? await _unitOfWork.Patients.GetByUserIdAsync(request.PatientId.ToString(), cancellationToken);

        var patientId = patient?.Id ?? request.PatientId;
        var appointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patientId, cancellationToken);
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }
}
