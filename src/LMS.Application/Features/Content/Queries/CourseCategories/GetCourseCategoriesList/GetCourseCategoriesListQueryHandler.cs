using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.CourseCategories.GetCourseCategoriesList;

public sealed class GetCourseCategoriesListQueryHandler : IRequestHandler<GetCourseCategoriesListQuery, ApiResult<List<CourseCategoryDto>>>
{
    private readonly ICourseCategoryRepository _courseCategoryRepository;
    private readonly IMapper _mapper;

    public GetCourseCategoriesListQueryHandler(
        ICourseCategoryRepository courseCategoryRepository,
        IMapper mapper)
    {
        _courseCategoryRepository = courseCategoryRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<CourseCategoryDto>>> Handle(GetCourseCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var courseCategories = await _courseCategoryRepository.GetAllOrderedAsync(cancellationToken);
        var courseCategoryDtos = _mapper.Map<List<CourseCategoryDto>>(courseCategories);
        return ApiResult<List<CourseCategoryDto>>.Ok(courseCategoryDtos);
    }
}
