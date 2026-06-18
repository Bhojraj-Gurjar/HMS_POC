namespace HMS.PatientRegistration.Application.Patients.DTOs;

public class DuplicateCheckRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? NationalId { get; set; }
    public string? Phone { get; set; }
}

public class DuplicateCheckResultDto
{
    public bool HasDuplicates { get; set; }
    public IReadOnlyList<PatientSummaryDto> Matches { get; set; } = Array.Empty<PatientSummaryDto>();
}
