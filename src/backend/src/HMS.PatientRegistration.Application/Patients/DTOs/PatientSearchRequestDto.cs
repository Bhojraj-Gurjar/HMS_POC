namespace HMS.PatientRegistration.Application.Patients.DTOs;

public class PatientSearchRequestDto
{
    public string? MrNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }
    public string? CivilId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
