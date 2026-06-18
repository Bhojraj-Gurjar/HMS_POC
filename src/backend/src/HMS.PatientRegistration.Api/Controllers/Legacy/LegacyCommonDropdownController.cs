using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Legacy.DTOs;
using HMS.PatientRegistration.Application.MasterData.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers.Legacy;

/// <summary>
/// Strangler-fig adapter for legacy CommonDropdown/Fetch endpoint.
/// </summary>
[Route("api/CommonDropdown")]
[ApiExplorerSettings(GroupName = "legacy")]
[Tags("Legacy Adapters")]
[AllowAnonymous]
public class LegacyCommonDropdownController : LegacyApiControllerBase
{
    private readonly IDropdownService _dropdownService;

    public LegacyCommonDropdownController(IDropdownService dropdownService)
    {
        _dropdownService = dropdownService;
    }

    /// <summary>
    /// Fetch one or more dropdown catalogs. Single type returns a flat array; multiple types return a batch object.
    /// </summary>
    [HttpPost("Fetch")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MasterDataItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LegacyDropdownBatchResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Fetch(
        [FromBody] LegacyCommonDropdownFetchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dropdownService.FetchLegacyAsync(request, cancellationToken);

        if (request.DropdownTypes is not { Count: > 0 } && !string.IsNullOrWhiteSpace(request.DropdownType))
        {
            var type = request.DropdownType.Trim();
            result.Dropdowns.TryGetValue(type, out var items);
            return OkResponse<object>(items ?? Array.Empty<MasterDataItemDto>());
        }

        return OkResponse<object>(result);
    }
}
