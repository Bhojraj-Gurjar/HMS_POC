using AutoMapper;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Patients.DTOs;
using MediatR;

namespace HMS.PatientRegistration.Application.Patients.Queries.SearchPatients;

public record SearchPatientsByCriteriaQuery(PatientSearchCriteria Criteria) : IRequest<PaginatedList<PatientSummaryDto>>;

public class SearchPatientsByCriteriaQueryHandler
    : IRequestHandler<SearchPatientsByCriteriaQuery, PaginatedList<PatientSummaryDto>>
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;

    public SearchPatientsByCriteriaQueryHandler(IPatientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PatientSummaryDto>> Handle(
        SearchPatientsByCriteriaQuery request,
        CancellationToken cancellationToken)
    {
        var criteria = request.Criteria;

        if (criteria.HasFieldCriteria())
        {
            var (items, total) = await _repository.SearchByCriteriaAsync(criteria, cancellationToken);
            return PaginatedList<PatientSummaryDto>.Create(
                _mapper.Map<List<PatientSummaryDto>>(items),
                total,
                criteria.PageNumber,
                criteria.PageSize);
        }

        var searchTerm = criteria.BuildCombinedSearchTerm();
        var (pageItems, totalCount) = await _repository.SearchAsync(
            searchTerm,
            criteria.PageNumber,
            criteria.PageSize,
            cancellationToken);

        return PaginatedList<PatientSummaryDto>.Create(
            _mapper.Map<List<PatientSummaryDto>>(pageItems),
            totalCount,
            criteria.PageNumber,
            criteria.PageSize);
    }
}
