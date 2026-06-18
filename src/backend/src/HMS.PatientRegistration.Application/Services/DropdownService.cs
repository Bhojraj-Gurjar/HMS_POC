using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Legacy.DTOs;
using HMS.PatientRegistration.Application.MasterData.DTOs;
using HMS.PatientRegistration.Application.MasterData.Queries.GetMasterDataByType;
using HMS.PatientRegistration.Domain.Exceptions;
using MediatR;

namespace HMS.PatientRegistration.Application.Services;

public class DropdownService : IDropdownService
{
    private readonly IMediator _mediator;

    public DropdownService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<IReadOnlyList<MasterDataItemDto>> GetByTypeAsync(
        string type,
        string? parentCode = null,
        CancellationToken cancellationToken = default) =>
        _mediator.Send(new GetMasterDataByTypeQuery(type, parentCode), cancellationToken);

    public async Task<LegacyDropdownBatchResultDto> FetchLegacyAsync(
        LegacyCommonDropdownFetchRequest request,
        CancellationToken cancellationToken = default)
    {
        var types = ResolveDropdownTypes(request);
        var dropdowns = new Dictionary<string, IReadOnlyList<MasterDataItemDto>>();

        foreach (var type in types)
        {
            dropdowns[type] = await GetByTypeAsync(type, cancellationToken: cancellationToken);
        }

        return new LegacyDropdownBatchResultDto { Dropdowns = dropdowns };
    }

    private static IReadOnlyList<string> ResolveDropdownTypes(LegacyCommonDropdownFetchRequest request)
    {
        if (request.DropdownTypes is { Count: > 0 })
        {
            return request.DropdownTypes
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.DropdownType))
        {
            return [request.DropdownType.Trim()];
        }

        throw new DomainException("DropdownType or DropdownTypes is required.");
    }
}
