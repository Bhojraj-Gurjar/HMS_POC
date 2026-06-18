using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Patients.DTOs;

namespace HMS.PatientRegistration.Application.Common.Interfaces.Services;

public interface IPatientRegistrationService
{
    Task<PatientDto> CreateAsync(CreatePatientDto request, CancellationToken cancellationToken = default);
    Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto request, CancellationToken cancellationToken = default);
    Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<PatientSummaryDto>> SearchAsync(
        PatientSearchCriteria criteria,
        CancellationToken cancellationToken = default);
    Task<DuplicateCheckResultDto> CheckDuplicatesAsync(
        DuplicateCheckRequestDto request,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
