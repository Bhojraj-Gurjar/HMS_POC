using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Legacy.DTOs;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Domain.Exceptions;

namespace HMS.PatientRegistration.Application.Services;

public class LegacyPatientRegistrationService : ILegacyPatientRegistrationService
{
    private readonly IPatientRegistrationService _patientRegistrationService;

    public LegacyPatientRegistrationService(IPatientRegistrationService patientRegistrationService)
    {
        _patientRegistrationService = patientRegistrationService;
    }

    public async Task<LegacyPatientIudResultDto> IudAsync(
        LegacyPatientIudRequest request,
        CancellationToken cancellationToken = default)
    {
        var operation = request.Operation.Trim().ToUpperInvariant();

        return operation switch
        {
            "I" => await InsertAsync(request, cancellationToken),
            "U" => await UpdateAsync(request, cancellationToken),
            "D" => await DeleteAsync(request, cancellationToken),
            _ => throw new DomainException($"Unsupported legacy operation '{request.Operation}'. Expected I, U, or D.")
        };
    }

    public Task<PaginatedList<PatientSummaryDto>> FetchAsync(
        LegacyPatientFetchRequest request,
        CancellationToken cancellationToken = default)
    {
        var criteria = new PatientSearchCriteria
        {
            MrNumber = request.MRNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MobileNumber = request.MobileNumber,
            CivilId = request.CivilID,
            SearchTerm = request.SearchTerm,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };

        return _patientRegistrationService.SearchAsync(criteria, cancellationToken);
    }

    public async Task<PatientDto> FetchPatientDataAsync(
        LegacyFetchPatientDataRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PatientID is { } patientId)
        {
            return await _patientRegistrationService.GetByIdAsync(patientId, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.MRNumber))
        {
            var criteria = new PatientSearchCriteria
            {
                MrNumber = request.MRNumber,
                PageNumber = 1,
                PageSize = 1,
            };

            var results = await _patientRegistrationService.SearchAsync(criteria, cancellationToken);
            var match = results.Items.FirstOrDefault()
                ?? throw new NotFoundException("Patient", request.MRNumber);

            return await _patientRegistrationService.GetByIdAsync(match.Id, cancellationToken);
        }

        throw new DomainException("PatientID or MRNumber is required for FetchPatientData.");
    }

    private async Task<LegacyPatientIudResultDto> InsertAsync(
        LegacyPatientIudRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PatientData is null)
        {
            throw new DomainException("PatientData is required for insert operation.");
        }

        var patient = await _patientRegistrationService.CreateAsync(request.PatientData, cancellationToken);
        return new LegacyPatientIudResultDto
        {
            Operation = "I",
            PatientID = patient.Id,
            Patient = patient,
        };
    }

    private async Task<LegacyPatientIudResultDto> UpdateAsync(
        LegacyPatientIudRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PatientID is not Guid patientId)
        {
            throw new DomainException("PatientID is required for update operation.");
        }

        if (request.UpdateData is null)
        {
            throw new DomainException("UpdateData is required for update operation.");
        }

        var patient = await _patientRegistrationService.UpdateAsync(patientId, request.UpdateData, cancellationToken);
        return new LegacyPatientIudResultDto
        {
            Operation = "U",
            PatientID = patient.Id,
            Patient = patient,
        };
    }

    private async Task<LegacyPatientIudResultDto> DeleteAsync(
        LegacyPatientIudRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PatientID is not Guid patientId)
        {
            throw new DomainException("PatientID is required for delete operation.");
        }

        await _patientRegistrationService.DeleteAsync(patientId, cancellationToken);
        return new LegacyPatientIudResultDto
        {
            Operation = "D",
            PatientID = patientId,
        };
    }
}
