using HMS.PatientRegistration.Domain.Entities;

namespace HMS.PatientRegistration.Application.Common.Mappings;

internal static class PatientMappingHelper
{
    public static string? GetPrimaryPhone(Patient patient)
    {
        var primary = patient.Contacts.FirstOrDefault(c => c.IsPrimary);
        if (primary is not null)
        {
            return primary.Value;
        }

        return patient.Contacts.FirstOrDefault()?.Value;
    }
}
