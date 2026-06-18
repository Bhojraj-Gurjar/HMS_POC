using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Patients.Commands.CreatePatient;
using HMS.PatientRegistration.Application.Patients.Commands.DeletePatient;
using HMS.PatientRegistration.Application.Patients.Commands.UpdatePatient;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Application.Patients.Queries.CheckDuplicate;
using HMS.PatientRegistration.Application.Patients.Queries.GetPatientById;
using HMS.PatientRegistration.Application.Patients.Queries.SearchPatients;
using MediatR;

namespace HMS.PatientRegistration.Application.Services;

public class PatientRegistrationService : IPatientRegistrationService
{
    private readonly IMediator _mediator;

    public PatientRegistrationService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<PatientDto> CreateAsync(CreatePatientDto request, CancellationToken cancellationToken = default) =>
        _mediator.Send(new CreatePatientCommand(request), cancellationToken);

    public Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto request, CancellationToken cancellationToken = default) =>
        _mediator.Send(new UpdatePatientCommand(id, request), cancellationToken);

    public Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _mediator.Send(new GetPatientByIdQuery(id), cancellationToken);

    public Task<PaginatedList<PatientSummaryDto>> SearchAsync(
        PatientSearchCriteria criteria,
        CancellationToken cancellationToken = default) =>
        _mediator.Send(new SearchPatientsByCriteriaQuery(criteria), cancellationToken);

    public Task<DuplicateCheckResultDto> CheckDuplicatesAsync(
        DuplicateCheckRequestDto request,
        CancellationToken cancellationToken = default) =>
        _mediator.Send(new CheckDuplicateQuery(request), cancellationToken);

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        _mediator.Send(new DeletePatientCommand(id), cancellationToken);
}
