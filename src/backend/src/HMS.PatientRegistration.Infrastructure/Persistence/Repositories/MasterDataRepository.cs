using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Repositories;

public class MasterDataRepository : IMasterDataRepository
{
    private readonly ApplicationDbContext _context;

    public MasterDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MasterDataItem>> GetByTypeAsync(
        MasterDataType type,
        string? parentCode = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.MasterDataItems
            .Where(m => m.Type == type && m.IsActive);

        if (!string.IsNullOrWhiteSpace(parentCode))
        {
            query = query.Where(m => m.ParentCode == parentCode);
        }
        else if (type is MasterDataType.State or MasterDataType.City or MasterDataType.Area)
        {
            return [];
        }

        return await query
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.DisplayName)
            .ToListAsync(cancellationToken);
    }
}
