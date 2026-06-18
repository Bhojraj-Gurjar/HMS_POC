using HMS.PatientRegistration.Application.Common.Helpers;
using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Patients.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers;

/// <summary>Modern REST patient CRUD, search, and duplicate detection.</summary>
// [Authorize] — enable when real JWT issuance is wired up
[AllowAnonymous]
[Tags("Patients")]
public class PatientsController : ApiControllerBase
{
    private readonly IPatientRegistrationService _patientRegistrationService;

    public PatientsController(IPatientRegistrationService patientRegistrationService)
    {
        _patientRegistrationService = patientRegistrationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PatientSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<PatientSummaryDto>>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var criteria = new PatientSearchCriteria
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
        var result = await _patientRegistrationService.SearchAsync(criteria, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}", Name = "GetPatientById")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _patientRegistrationService.GetByIdAsync(id, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create(
        [FromBody] CreatePatientDto request,
        CancellationToken cancellationToken)
    {
        var result = await _patientRegistrationService.CreateAsync(request, cancellationToken);
        return CreatedResponse("GetPatientById", new { id = result.Id }, result, "Patient registered successfully.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Update(
        Guid id,
        [FromBody] UpdatePatientDto request,
        CancellationToken cancellationToken)
    {
        var result = await _patientRegistrationService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result, "Patient updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _patientRegistrationService.DeleteAsync(id, cancellationToken);
        object payload = new { id };
        return OkResponse(payload, "Patient deleted successfully.");
    }

    [HttpPost("duplicate-check")]
    [ProducesResponseType(typeof(ApiResponse<DuplicateCheckResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DuplicateCheckResultDto>>> CheckDuplicate(
        [FromBody] DuplicateCheckRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _patientRegistrationService.CheckDuplicatesAsync(request, cancellationToken);
        return OkResponse(result);
    }
}
