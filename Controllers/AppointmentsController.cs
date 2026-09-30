using ClinicSystem.Application.Features.Appointments.Commands.CancelAppointment;
using ClinicSystem.Application.Features.Appointments.Commands.CompleteAppointment;
using ClinicSystem.Application.Features.Appointments.Commands.ConfirmAppointment;
using ClinicSystem.Application.Features.Appointments.Commands.CreateAppointment;
using ClinicSystem.Application.Features.Appointments.Queries.GetAppointmentById;
using ClinicSystem.Application.Features.Appointments.Queries.GetPatientAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
    {
        var result = await _mediator.Send(command);
        return Created(string.Empty, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(Guid id)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery(id));
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientAppointments(Guid patientId)
    {
        var result = await _mediator.Send(new GetPatientAppointmentsQuery(patientId));
        return Ok(result);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        await _mediator.Send(new CancelAppointmentCommand(id));
        return NoContent();
    }

    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(Guid id)
    {
        await _mediator.Send(new ConfirmAppointmentCommand(id));
        return NoContent();
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteAppointment(Guid id)
    {
        await _mediator.Send(new CompleteAppointmentCommand(id));
        return NoContent();
    }
}
