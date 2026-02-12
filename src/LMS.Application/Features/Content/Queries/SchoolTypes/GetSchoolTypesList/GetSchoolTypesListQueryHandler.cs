using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.SchoolTypes.GetSchoolTypesList;

public sealed class GetSchoolTypesListQueryHandler : IRequestHandler<GetSchoolTypesListQuery, ApiResult<List<SchoolTypeDto>>>
{
    private readonly ISchoolTypeRepository _schoolTypeRepository;
    private readonly IMapper _mapper;

    public GetSchoolTypesListQueryHandler(
        ISchoolTypeRepository schoolTypeRepository,
        IMapper mapper)
    {
        _schoolTypeRepository = schoolTypeRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<SchoolTypeDto>>> Handle(GetSchoolTypesListQuery request, CancellationToken cancellationToken)
    {
        var schoolTypes = await _schoolTypeRepository.GetAllOrderedAsync(cancellationToken);
        var schoolTypeDtos = _mapper.Map<List<SchoolTypeDto>>(schoolTypes);
        return ApiResult<List<SchoolTypeDto>>.Ok(schoolTypeDtos);
    }
}
