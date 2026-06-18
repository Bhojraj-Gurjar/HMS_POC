using AutoMapper;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Patients.DTOs;
using MediatR;

namespace HMS.PatientRegistration.Application.Patients.Queries.CheckDuplicate;

public record CheckDuplicateQuery(DuplicateCheckRequestDto Request) : IRequest<DuplicateCheckResultDto>;

public class CheckDuplicateQueryHandler : IRequestHandler<CheckDuplicateQuery, DuplicateCheckResultDto>
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;

    public CheckDuplicateQueryHandler(IPatientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DuplicateCheckResultDto> Handle(
        CheckDuplicateQuery request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        var matches = await _repository.FindPotentialDuplicatesAsync(
            dto.FirstName,
            dto.LastName,
            dto.DateOfBirth,
            dto.NationalId,
            dto.Phone,
            cancellationToken);

        var summaries = _mapper.Map<List<PatientSummaryDto>>(matches);
        return new DuplicateCheckResultDto
        {
            HasDuplicates = summaries.Count > 0,
            Matches = summaries,
        };
    }
}
