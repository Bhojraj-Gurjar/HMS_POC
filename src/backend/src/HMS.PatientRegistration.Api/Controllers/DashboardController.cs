using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Dashboard.DTOs;
using HMS.PatientRegistration.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers;

[AllowAnonymous]
public class DashboardController : ApiControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetStatsAsync(cancellationToken);
        return OkResponse(result);
    }
}
