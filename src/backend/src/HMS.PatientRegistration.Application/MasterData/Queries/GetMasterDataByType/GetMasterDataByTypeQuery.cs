using HMS.PatientRegistration.Application.Common.Helpers;
using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Domain.Enums;
using HMS.PatientRegistration.Domain.Exceptions;
using MediatR;

namespace HMS.PatientRegistration.Application.MasterData.Queries.GetMasterDataByType;

public record GetMasterDataByTypeQuery(string Type, string? ParentCode = null) : IRequest<IReadOnlyList<MasterData.DTOs.MasterDataItemDto>>;

public class GetMasterDataByTypeQueryHandler : IRequestHandler<GetMasterDataByTypeQuery, IReadOnlyList<MasterData.DTOs.MasterDataItemDto>>
{
    private readonly IMasterDataRepository _repository;
    private readonly AutoMapper.IMapper _mapper;

    public GetMasterDataByTypeQueryHandler(IMasterDataRepository repository, AutoMapper.IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MasterData.DTOs.MasterDataItemDto>> Handle(
        GetMasterDataByTypeQuery request,
        CancellationToken cancellationToken)
    {
        var type = EnumParser.ParseMasterDataType(request.Type);
        var items = await _repository.GetByTypeAsync(type, request.ParentCode, cancellationToken);
        return _mapper.Map<List<MasterData.DTOs.MasterDataItemDto>>(items);
    }
}
