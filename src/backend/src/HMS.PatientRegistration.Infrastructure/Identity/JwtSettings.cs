namespace HMS.PatientRegistration.Infrastructure.Identity;

/// <summary>
/// JWT configuration. Override SecretKey via environment variable Jwt__SecretKey in production.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";
    private const int MinimumSecretKeyLength = 32;

    public string Issuer { get; set; } = "HMS.PatientRegistration";
    public string Audience { get; set; } = "HMS.PatientRegistration.Client";
    public string SecretKey { get; set; } = "REPLACE_WITH_A_SECURE_KEY_AT_LEAST_32_CHARS_LONG";
    public int ExpirationMinutes { get; set; } = 60;

    public void Validate(bool isDevelopment)
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
        {
            throw new InvalidOperationException("Jwt:SecretKey is required.");
        }

        if (SecretKey.Length < MinimumSecretKeyLength)
        {
            throw new InvalidOperationException(
                $"Jwt:SecretKey must be at least {MinimumSecretKeyLength} characters.");
        }

        if (!isDevelopment && SecretKey.StartsWith("REPLACE_", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Jwt:SecretKey placeholder must be replaced before running in non-development environments.");
        }
    }
}
