using HMS.PatientRegistration.Application.Auth.Commands.Login;
using HMS.PatientRegistration.Application.Auth.DTOs;
using HMS.PatientRegistration.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

  /// <summary>
    /// JWT-ready login placeholder. Returns a mock token for POC/demo.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request), cancellationToken);
        return OkResponse(result, "Login successful (placeholder token).");
    }
}
