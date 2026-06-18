namespace HMS.PatientRegistration.Application.Common.Interfaces;

public interface IRuntimeDataMode
{
    string ConfiguredMode { get; }
    string EffectiveMode { get; }
    bool IsMockFallbackActive { get; }
}
