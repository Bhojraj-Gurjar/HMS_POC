namespace HMS.PatientRegistration.Application.Patients.DTOs;

public class PatientSummaryDto
{
    public Guid Id { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PrimaryPhone { get; set; }
    public DateTime CreatedAt { get; set; }
}
