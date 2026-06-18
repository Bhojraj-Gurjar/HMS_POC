using HMS.PatientRegistration.Application.Legacy.DTOs;
using HMS.PatientRegistration.Application.MasterData.DTOs;

namespace HMS.PatientRegistration.Application.Common.Interfaces.Services;

public interface IDropdownService
{
    Task<IReadOnlyList<MasterDataItemDto>> GetByTypeAsync(
        string type,
        string? parentCode = null,
        CancellationToken cancellationToken = default);
    Task<LegacyDropdownBatchResultDto> FetchLegacyAsync(
        LegacyCommonDropdownFetchRequest request,
        CancellationToken cancellationToken = default);
}
