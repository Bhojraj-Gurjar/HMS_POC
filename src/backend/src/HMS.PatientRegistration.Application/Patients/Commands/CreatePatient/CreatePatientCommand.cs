using AutoMapper;
using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Application.Patients.Helpers;
using HMS.PatientRegistration.Domain.Exceptions;
using MediatR;

namespace HMS.PatientRegistration.Application.Patients.Commands.CreatePatient;

public record CreatePatientCommand(CreatePatientDto Request) : IRequest<PatientDto>;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
{
    private readonly IPatientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreatePatientCommandHandler(
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

    public async Task<PatientDto> Handle(CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var phone = PatientEntityFactory.GetPrimaryPhone(request);

        if (!request.AllowDuplicateOverride)
        {
            var duplicates = await _repository.FindPotentialDuplicatesAsync(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.NationalId,
                phone,
                cancellationToken);

            if (duplicates.Count > 0)
            {
                throw new DuplicatePatientException("A patient with similar details already exists.")
                {
                    MatchingPatientIds = duplicates.Select(d => d.Id).ToList(),
                };
            }
        }

        var id = Guid.NewGuid();
        var patientNumber = await _repository.GenerateNextPatientNumberAsync(cancellationToken);
        var now = _dateTimeProvider.UtcNow;
        var user = _currentUser.UserName ?? "system";

        var patient = PatientEntityFactory.CreateFromDto(request, id, patientNumber, now, user);
        await _repository.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PatientDto>(patient);
    }
}
