namespace HMS.PatientRegistration.Domain.Entities;

public class PatientInsurance : Common.BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string ProviderName { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public bool IsPrimary { get; set; }
}
