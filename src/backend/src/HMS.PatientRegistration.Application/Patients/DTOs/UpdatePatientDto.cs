namespace HMS.PatientRegistration.Application.Patients.DTOs;

public class UpdatePatientDto
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
    public IReadOnlyList<UpdatePatientAddressDto> Addresses { get; set; } = Array.Empty<UpdatePatientAddressDto>();
    public IReadOnlyList<UpdatePatientContactDto> Contacts { get; set; } = Array.Empty<UpdatePatientContactDto>();
    public IReadOnlyList<UpdatePatientInsuranceDto> Insurances { get; set; } = Array.Empty<UpdatePatientInsuranceDto>();
    public IReadOnlyList<UpdateEmergencyContactDto> EmergencyContacts { get; set; } = Array.Empty<UpdateEmergencyContactDto>();
}

public class UpdatePatientAddressDto
{
    public Guid? Id { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class UpdatePatientContactDto
{
    public Guid? Id { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class UpdatePatientInsuranceDto
{
    public Guid? Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public bool IsPrimary { get; set; }
}

public class UpdateEmergencyContactDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
}
