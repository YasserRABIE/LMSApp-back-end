using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Courses.GetCourseById;

public sealed record GetCourseByIdQuery(
    Guid CourseId
) : IRequest<ApiResult<CourseDto>>;
