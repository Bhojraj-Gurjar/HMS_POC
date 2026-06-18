using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Domain.Entities;

public class PatientAddress : Common.BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public AddressType AddressType { get; set; }
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
