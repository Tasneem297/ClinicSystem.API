using ClinicSystem.Application.Features.Auth.Commands.Login;
using ClinicSystem.Application.Features.Auth.Commands.RegisterDoctor;
using ClinicSystem.Application.Features.Auth.Commands.RegisterPatient;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register/patient")]
    public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("register/doctor")]
    public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
