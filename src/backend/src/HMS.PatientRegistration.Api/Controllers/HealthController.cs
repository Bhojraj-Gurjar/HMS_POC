using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers;

/// <summary>Health check and active data mode (Mock vs SQL Server).</summary>
[AllowAnonymous]
[Tags("Health")]
public class HealthController : ApiControllerBase
{
    private readonly IRuntimeDataMode _runtimeDataMode;

    public HealthController(IRuntimeDataMode runtimeDataMode)
    {
        _runtimeDataMode = runtimeDataMode;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<object>> Get()
    {
        var payload = new
        {
            status = "Healthy",
            dataMode = _runtimeDataMode.EffectiveMode,
            configuredDataMode = _runtimeDataMode.ConfiguredMode,
            mockFallbackActive = _runtimeDataMode.IsMockFallbackActive,
            timestamp = DateTime.UtcNow
        };

        return OkResponse<object>(payload);
    }
}
