namespace HMS.PatientRegistration.Application.MasterData.DTOs;

public class MasterDataItemDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
    public int SortOrder { get; set; }
}
