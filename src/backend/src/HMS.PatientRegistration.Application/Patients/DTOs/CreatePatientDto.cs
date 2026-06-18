namespace HMS.PatientRegistration.Application.Patients.DTOs;

public class CreatePatientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Nationality { get; set; }
    public string? LegacyPatientId { get; set; }
    public string? Notes { get; set; }
    public bool AllowDuplicateOverride { get; set; }
    public IReadOnlyList<CreatePatientAddressDto> Addresses { get; set; } = Array.Empty<CreatePatientAddressDto>();
    public IReadOnlyList<CreatePatientContactDto> Contacts { get; set; } = Array.Empty<CreatePatientContactDto>();
    public IReadOnlyList<CreatePatientInsuranceDto> Insurances { get; set; } = Array.Empty<CreatePatientInsuranceDto>();
    public IReadOnlyList<CreateEmergencyContactDto> EmergencyContacts { get; set; } = Array.Empty<CreateEmergencyContactDto>();
}

public class CreatePatientAddressDto
{
    public string AddressType { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class CreatePatientContactDto
{
    public string ContactType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class CreatePatientInsuranceDto
{
    public string ProviderName { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public bool IsPrimary { get; set; }
}

public class CreateEmergencyContactDto
{
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
}
