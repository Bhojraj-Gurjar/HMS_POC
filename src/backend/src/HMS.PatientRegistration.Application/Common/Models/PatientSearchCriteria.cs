namespace HMS.PatientRegistration.Application.Common.Models;

/// <summary>
/// Field-level patient search criteria used by repositories and search endpoints.
/// </summary>
public class PatientSearchCriteria
{
    public string? MrNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }
    public string? CivilId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string? BuildCombinedSearchTerm()
    {
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            return SearchTerm.Trim();
        }

        var parts = new[] { MrNumber, FirstName, LastName, MobileNumber, CivilId }
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p!.Trim());

        var combined = string.Join(' ', parts);
        return string.IsNullOrWhiteSpace(combined) ? null : combined;
    }

    public bool HasFieldCriteria() =>
        !string.IsNullOrWhiteSpace(MrNumber) ||
        !string.IsNullOrWhiteSpace(FirstName) ||
        !string.IsNullOrWhiteSpace(LastName) ||
        !string.IsNullOrWhiteSpace(MobileNumber) ||
        !string.IsNullOrWhiteSpace(CivilId);
}
