using HMS.PatientRegistration.Application.Patients.DTOs;

using HMS.PatientRegistration.Application.MasterData.DTOs;

namespace HMS.PatientRegistration.Application.Legacy.DTOs;

public class LegacyPatientIudRequest
{
    /// <summary>Legacy operation code: I (insert), U (update), D (delete).</summary>
    public string Operation { get; set; } = "I";

    public Guid? PatientID { get; set; }
    public CreatePatientDto? PatientData { get; set; }
    public UpdatePatientDto? UpdateData { get; set; }
}

public class LegacyPatientIudResultDto
{
    public string Operation { get; set; } = string.Empty;
    public Guid? PatientID { get; set; }
    public PatientDto? Patient { get; set; }
}

public class LegacyPatientFetchRequest
{
    public string? MRNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }
    public string? CivilID { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class LegacyFetchPatientDataRequest
{
    public Guid? PatientID { get; set; }
    public string? MRNumber { get; set; }
}

public class LegacyCommonDropdownFetchRequest
{
    public string? DropdownType { get; set; }
    public IReadOnlyList<string>? DropdownTypes { get; set; }
}

public class LegacyDropdownBatchResultDto
{
    public IReadOnlyDictionary<string, IReadOnlyList<MasterDataItemDto>> Dropdowns { get; set; }
        = new Dictionary<string, IReadOnlyList<MasterDataItemDto>>();
}
