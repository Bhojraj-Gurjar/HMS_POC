namespace HMS.PatientRegistration.Domain.Exceptions;

public class DuplicatePatientException : DomainException
{
    public DuplicatePatientException(string message) : base(message)
    {
    }

    public IReadOnlyList<Guid> MatchingPatientIds { get; init; } = Array.Empty<Guid>();
}
