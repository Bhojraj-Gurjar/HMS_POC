using HMS.PatientRegistration.Application.Common.Helpers;
using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Patients.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers;

[Route("api/patient-registration")]
[AllowAnonymous]
[Tags("Patient Registration")]
public class PatientRegistrationController : ApiControllerBase
{
    private readonly IPatientRegistrationService _patientRegistrationService;

    public PatientRegistrationController(IPatientRegistrationService patientRegistrationService)
    {
        _patientRegistrationService = patientRegistrationService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create(
        [FromBody] CreatePatientDto request,
        CancellationToken cancellationToken)
    {
        var result = await _patientRegistrationService.CreateAsync(request, cancellationToken);
        return CreatedResponse("GetPatientRegistrationById", new { id = result.Id }, result, "Patient registered successfully.");
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

    [HttpGet("{id:guid}", Name = "GetPatientRegistrationById")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _patientRegistrationService.GetByIdAsync(id, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PatientSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<PatientSummaryDto>>>> Search(
        [FromBody] PatientSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var criteria = PatientSearchCriteriaMapper.FromRequest(request);
        var result = await _patientRegistrationService.SearchAsync(criteria, cancellationToken);
        return OkResponse(result);
    }
}
