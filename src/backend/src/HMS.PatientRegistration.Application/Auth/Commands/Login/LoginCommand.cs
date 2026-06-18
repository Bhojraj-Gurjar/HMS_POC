using HMS.PatientRegistration.Application.Auth.DTOs;
using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using MediatR;

namespace HMS.PatientRegistration.Application.Auth.Commands.Login;

public record LoginCommand(LoginRequestDto Request) : IRequest<LoginResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var username = string.IsNullOrWhiteSpace(command.Request.Username)
            ? "demo"
            : command.Request.Username.Trim();

        var roles = new[] { "Receptionist", "Admin" };
        var (accessToken, expiresInSeconds) = _jwtTokenGenerator.GenerateAccessToken(username, roles);

        return Task.FromResult(new LoginResponseDto
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresInSeconds = expiresInSeconds,
            Username = username,
            Roles = roles,
        });
    }
}