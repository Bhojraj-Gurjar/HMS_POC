namespace HMS.PatientRegistration.Application.Patients.DTOs;

public class PatientDto
{
    public Guid Id { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Nationality { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? LegacyPatientId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<PatientAddressDto> Addresses { get; set; } = Array.Empty<PatientAddressDto>();
    public IReadOnlyList<PatientContactDto> Contacts { get; set; } = Array.Empty<PatientContactDto>();
    public IReadOnlyList<PatientInsuranceDto> Insurances { get; set; } = Array.Empty<PatientInsuranceDto>();
    public IReadOnlyList<EmergencyContactDto> EmergencyContacts { get; set; } = Array.Empty<EmergencyContactDto>();
}
