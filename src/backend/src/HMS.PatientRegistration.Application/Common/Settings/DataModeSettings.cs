namespace HMS.PatientRegistration.Application.Common.Settings;

public class DataModeSettings
{
    public const string SectionName = "DataMode";

    /// <summary>
    /// Supported values: "Mock", "Sqlite", "Database", or "SqlServer".
    /// </summary>
    public string Mode { get; set; } = "Sqlite";

    public bool IsMock => string.Equals(Mode, "Mock", StringComparison.OrdinalIgnoreCase);

    public bool IsSqlite => string.Equals(Mode, "Sqlite", StringComparison.OrdinalIgnoreCase);

    public bool IsDatabase =>
        string.Equals(Mode, "Database", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Mode, "SqlServer", StringComparison.OrdinalIgnoreCase);

    public bool UsesEfCore => IsSqlite || IsDatabase;

    /// <summary>
    /// When true and SQL Server is unreachable at startup, automatically use in-memory mock repositories.
    /// </summary>
    public bool AllowMockFallback { get; set; } = true;
}
