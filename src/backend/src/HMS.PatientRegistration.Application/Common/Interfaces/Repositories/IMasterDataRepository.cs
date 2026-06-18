using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Application.Common.Interfaces.Repositories;

public interface IMasterDataRepository
{
    Task<IReadOnlyList<MasterDataItem>> GetByTypeAsync(
        MasterDataType type,
        string? parentCode = null,
        CancellationToken cancellationToken = default);
}
