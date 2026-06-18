using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Domain.Exceptions;
using MediatR;

namespace HMS.PatientRegistration.Application.Patients.Commands.DeletePatient;

public record DeletePatientCommand(Guid Id) : IRequest;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand>
{
    private readonly IPatientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeletePatientCommandHandler(
        IPatientRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(DeletePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Patient", command.Id);

        patient.IsDeleted = true;
        patient.DeletedAt = _dateTimeProvider.UtcNow;
        patient.DeletedBy = _currentUser.UserName ?? "system";

        await _repository.SoftDeleteAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
