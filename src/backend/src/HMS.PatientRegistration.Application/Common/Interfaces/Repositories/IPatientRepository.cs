using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Domain.Entities;

namespace HMS.PatientRegistration.Application.Common.Interfaces.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Patient?> GetByPatientNumberAsync(string patientNumber, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchByCriteriaAsync(
        PatientSearchCriteria criteria,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> FindPotentialDuplicatesAsync(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string? nationalId,
        string? phone,
        CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<string> GenerateNextPatientNumberAsync(CancellationToken cancellationToken = default);
}
