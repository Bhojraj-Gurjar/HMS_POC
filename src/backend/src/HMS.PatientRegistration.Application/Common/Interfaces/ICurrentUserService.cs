namespace HMS.PatientRegistration.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
}
