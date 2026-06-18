using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Legacy.DTOs;
using HMS.PatientRegistration.Application.Patients.DTOs;

namespace HMS.PatientRegistration.Application.Common.Interfaces.Services;

public interface ILegacyPatientRegistrationService
{
    Task<LegacyPatientIudResultDto> IudAsync(
        LegacyPatientIudRequest request,
        CancellationToken cancellationToken = default);
    Task<PaginatedList<PatientSummaryDto>> FetchAsync(
        LegacyPatientFetchRequest request,
        CancellationToken cancellationToken = default);
    Task<PatientDto> FetchPatientDataAsync(
        LegacyFetchPatientDataRequest request,
        CancellationToken cancellationToken = default);
}
