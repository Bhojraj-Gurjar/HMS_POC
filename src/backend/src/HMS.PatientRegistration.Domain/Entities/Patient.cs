using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Domain.Entities;

public class Patient : Common.AuditableEntity
{
    public string PatientNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public BloodGroup BloodGroup { get; set; }
    public string? NationalId { get; set; }
    public string? Nationality { get; set; }
    public PatientStatus Status { get; set; } = PatientStatus.Active;
    public string? LegacyPatientId { get; set; }
    public string? Notes { get; set; }

    public ICollection<PatientAddress> Addresses { get; set; } = new List<PatientAddress>();
    public ICollection<PatientContact> Contacts { get; set; } = new List<PatientContact>();
    public ICollection<PatientInsurance> Insurances { get; set; } = new List<PatientInsurance>();
    public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

    public string FullName => string.Join(" ",
        new[] { FirstName, MiddleName, LastName }.Where(n => !string.IsNullOrWhiteSpace(n)));
}
