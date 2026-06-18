using HMS.PatientRegistration.Application.Common.Interfaces;

namespace HMS.PatientRegistration.Infrastructure.Services;

public sealed class RuntimeDataMode : IRuntimeDataMode
{
    public required string ConfiguredMode { get; init; }
    public required string EffectiveMode { get; init; }
    public required bool IsMockFallbackActive { get; init; }
}
