using ClinicSystem.Application.Features.Doctors.Commands.AddDoctorSchedule;
using ClinicSystem.Application.Features.Doctors.Queries.GetAvailableSlots;
using ClinicSystem.Application.Features.Doctors.Queries.GetDoctorSchedule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSystem.API.Controllers;

public record AddDoctorScheduleRequest(DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{doctorId}/schedule")]
    public async Task<IActionResult> AddSchedule(Guid doctorId, [FromBody] AddDoctorScheduleRequest request)
    {
        var result = await _mediator.Send(new AddDoctorScheduleCommand(doctorId, request.DayOfWeek, request.StartTime, request.EndTime));
        return Created(string.Empty, result);
    }

    [HttpGet("{doctorId}/schedule")]
    public async Task<IActionResult> GetSchedule(Guid doctorId)
    {
        var result = await _mediator.Send(new GetDoctorScheduleQuery(doctorId));
        return Ok(result);
    }

    [HttpGet("{doctorId}/available-slots")]
    public async Task<IActionResult> GetAvailableSlots(Guid doctorId, [FromQuery] DateTime date)
    {
        var result = await _mediator.Send(new GetAvailableSlotsQuery(doctorId, date));
        return Ok(result);
    }
}
