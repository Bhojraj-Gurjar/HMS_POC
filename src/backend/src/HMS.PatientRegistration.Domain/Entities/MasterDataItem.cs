using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Domain.Entities;

public class MasterDataItem : Common.BaseEntity
{
    public MasterDataType Type { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
