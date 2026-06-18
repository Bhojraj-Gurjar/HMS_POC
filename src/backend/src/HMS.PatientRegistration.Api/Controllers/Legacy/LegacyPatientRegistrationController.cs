using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Legacy.DTOs;
using HMS.PatientRegistration.Application.Patients.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers.Legacy;

/// <summary>
/// Strangler-fig adapter for legacy MVC patient registration endpoints.
/// </summary>
[Route("api/patientregistration")]
[ApiExplorerSettings(GroupName = "legacy")]
[Tags("Legacy Adapters")]
[AllowAnonymous]
public class LegacyPatientRegistrationController : LegacyApiControllerBase
{
    private readonly ILegacyPatientRegistrationService _legacyService;

    public LegacyPatientRegistrationController(ILegacyPatientRegistrationService legacyService)
    {
        _legacyService = legacyService;
    }

    /// <summary>Legacy insert / update / delete multiplexer (operation I, U, or D).</summary>
    [HttpPost("IUD")]
    [ProducesResponseType(typeof(ApiResponse<LegacyPatientIudResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LegacyPatientIudResultDto>>> Iud(
        [FromBody] LegacyPatientIudRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _legacyService.IudAsync(request, cancellationToken);
        return OkResponse(result);
    }

    /// <summary>Legacy patient search with field-level filters.</summary>
    [HttpPost("Fetch")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PatientSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<PatientSummaryDto>>>> Fetch(
        [FromBody] LegacyPatientFetchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _legacyService.FetchAsync(request, cancellationToken);
        return OkResponse(result);
    }

    /// <summary>Load a single patient by GUID or MR number.</summary>
    [HttpPost("FetchPatientData")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> FetchPatientData(
        [FromBody] LegacyFetchPatientDataRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _legacyService.FetchPatientDataAsync(request, cancellationToken);
        return OkResponse(result);
    }
}
