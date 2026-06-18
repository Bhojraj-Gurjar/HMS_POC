namespace HMS.PatientRegistration.Application.Common.Interfaces.Repositories;

public interface IStoredProcedurePatientRepository
{
    Task<Guid> InsertOrUpdatePatientAsync(object request, CancellationToken cancellationToken = default);
    Task<object?> FetchPatientAsync(object request, CancellationToken cancellationToken = default);
    Task<object?> FetchPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default);
}
