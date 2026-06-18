using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Domain.Enums;
using HMS.PatientRegistration.Infrastructure.Persistence.Seed;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Repositories;

public class MockMasterDataRepository : IMasterDataRepository
{
    private static readonly IReadOnlyList<MasterDataItem> Items = MockSeedData.CreateMasterDataItems();

    public Task<IReadOnlyList<MasterDataItem>> GetByTypeAsync(
        MasterDataType type,
        string? parentCode = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<MasterDataItem>>(
            Items
                .Where(i => i.Type == type && i.IsActive && MatchesParent(i, type, parentCode))
                .OrderBy(i => i.SortOrder)
                .ToList());

    private static bool MatchesParent(MasterDataItem item, MasterDataType type, string? parentCode)
    {
        if (type is MasterDataType.State or MasterDataType.City or MasterDataType.Area)
        {
            return !string.IsNullOrWhiteSpace(parentCode)
                && string.Equals(item.ParentCode, parentCode, StringComparison.OrdinalIgnoreCase);
        }

        return string.IsNullOrWhiteSpace(parentCode)
            || string.Equals(item.ParentCode, parentCode, StringComparison.OrdinalIgnoreCase);
    }
}
