using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Domain.Entities;

public class PatientContact : Common.BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public ContactType ContactType { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
