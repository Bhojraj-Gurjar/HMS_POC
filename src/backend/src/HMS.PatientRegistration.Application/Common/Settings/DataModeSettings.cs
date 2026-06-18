namespace HMS.PatientRegistration.Application.Common.Settings;

public class DataModeSettings
{
    public const string SectionName = "DataMode";

    /// <summary>
    /// Supported values: "Mock", "Database", or "SqlServer".
    /// </summary>
    public string Mode { get; set; } = "Mock";

    public bool IsMock => string.Equals(Mode, "Mock", StringComparison.OrdinalIgnoreCase);

    public bool IsDatabase =>
        string.Equals(Mode, "Database", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Mode, "SqlServer", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// When true and SQL Server is unreachable at startup, automatically use in-memory mock repositories.
    /// </summary>
    public bool AllowMockFallback { get; set; } = true;
}
