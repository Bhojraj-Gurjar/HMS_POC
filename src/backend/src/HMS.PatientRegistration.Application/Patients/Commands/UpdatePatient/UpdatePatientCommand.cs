using AutoMapper;
using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Application.Patients.Helpers;
using HMS.PatientRegistration.Domain.Exceptions;
using MediatR;

namespace HMS.PatientRegistration.Application.Patients.Commands.UpdatePatient;

public record UpdatePatientCommand(Guid Id, UpdatePatientDto Request) : IRequest<PatientDto>;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientDto>
{
    private readonly IPatientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdatePatientCommandHandler(
        IPatientRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PatientDto> Handle(UpdatePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Patient", command.Id);

        PatientEntityFactory.ApplyUpdate(
            patient,
            command.Request,
            _dateTimeProvider.UtcNow,
            _currentUser.UserName ?? "system");

        await _repository.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PatientDto>(patient);
    }
}
