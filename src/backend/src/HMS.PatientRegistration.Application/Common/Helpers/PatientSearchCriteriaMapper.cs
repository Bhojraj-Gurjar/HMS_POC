using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Domain.Entities;

namespace HMS.PatientRegistration.Application.Common.Helpers;

public static class PatientSearchCriteriaMapper
{
    public static PatientSearchCriteria FromRequest(PatientSearchRequestDto request) => new()
    {
        MrNumber = request.MrNumber,
        FirstName = request.FirstName,
        LastName = request.LastName,
        MobileNumber = request.MobileNumber,
        CivilId = request.CivilId,
        SearchTerm = request.SearchTerm,
        PageNumber = request.PageNumber,
        PageSize = request.PageSize,
    };

    public static bool Matches(Patient patient, PatientSearchCriteria criteria)
    {
        return Includes(patient.PatientNumber, criteria.MrNumber)
               && Includes(patient.FirstName, criteria.FirstName)
               && Includes(patient.LastName, criteria.LastName)
               && Includes(
                   patient.Contacts.FirstOrDefault(c => c.IsPrimary)?.Value
                   ?? patient.Contacts.FirstOrDefault()?.Value,
                   criteria.MobileNumber)
               && Includes(patient.NationalId, criteria.CivilId);
    }

    private static bool Includes(string? value, string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return true;
        }

        return (value ?? string.Empty).Contains(term.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
