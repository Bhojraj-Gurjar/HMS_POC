namespace HMS.PatientRegistration.Application.Common.Interfaces.Services;

public interface IJwtTokenGenerator
{
    (string AccessToken, int ExpiresInSeconds) GenerateAccessToken(
        string username,
        IEnumerable<string> roles);
}
