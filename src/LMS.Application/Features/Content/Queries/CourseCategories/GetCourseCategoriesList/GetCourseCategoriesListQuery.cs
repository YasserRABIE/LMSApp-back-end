using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.CourseCategories.GetCourseCategoriesList;

public sealed record GetCourseCategoriesListQuery : IRequest<ApiResult<List<CourseCategoryDto>>>;
