using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace HMS.PatientRegistration.Infrastructure.Persistence.StoredProcedures;

/// <summary>
/// Placeholder implementation until legacy SQL stored procedures are wired.
/// </summary>
public sealed class StoredProcedurePatientRepository : IStoredProcedurePatientRepository
{
    private readonly ILogger<StoredProcedurePatientRepository> _logger;

    public StoredProcedurePatientRepository(ILogger<StoredProcedurePatientRepository> logger)
    {
        _logger = logger;
    }

    public Task<Guid> InsertOrUpdatePatientAsync(object request, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "Stored procedure Insert/Update patient is not yet wired. Use EF Core repository in POC mode.");
        throw new NotImplementedException(
            "SP integration placeholder: wire usp_PatientRegistration_IUD during SQL Server migration.");
    }

    public Task<object?> FetchPatientAsync(object request, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Stored procedure Fetch patient is not yet wired.");
        throw new NotImplementedException(
            "SP integration placeholder: wire usp_PatientRegistration_Fetch during SQL Server migration.");
    }

    public Task<object?> FetchPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Stored procedure FetchPatientById is not yet wired for {PatientId}.", patientId);
        throw new NotImplementedException(
            "SP integration placeholder: wire usp_PatientRegistration_FetchById during SQL Server migration.");
    }
}
