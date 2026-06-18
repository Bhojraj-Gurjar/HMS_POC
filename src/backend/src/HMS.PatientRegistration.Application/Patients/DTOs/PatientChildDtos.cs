namespace HMS.PatientRegistration.Application.Patients.DTOs;

public class PatientAddressDto
{
    public Guid Id { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class PatientContactDto
{
    public Guid Id { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class PatientInsuranceDto
{
    public Guid Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public bool IsPrimary { get; set; }
}

public class EmergencyContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
}
