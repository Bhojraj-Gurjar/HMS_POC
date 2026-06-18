using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.MasterData.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers;

[Route("api/[controller]")]
[Route("api/dropdowns")]
[AllowAnonymous]
public class MasterDataController : ApiControllerBase
{
    private readonly IDropdownService _dropdownService;

    public MasterDataController(IDropdownService dropdownService)
    {
        _dropdownService = dropdownService;
    }

    [HttpGet("{type}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MasterDataItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MasterDataItemDto>>>> GetByType(
        string type,
        [FromQuery] string? parentCode,
        CancellationToken cancellationToken)
    {
        var result = await _dropdownService.GetByTypeAsync(type, parentCode, cancellationToken);
        return OkResponse(result);
    }
}
