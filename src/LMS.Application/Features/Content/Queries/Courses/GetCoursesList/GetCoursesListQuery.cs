using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Courses.GetCoursesList;

public sealed record GetCoursesListQuery(
    Guid? TeacherId = null,
    Guid? SubjectId = null,
    bool PublishedOnly = false
) : IRequest<ApiResult<List<CourseListDto>>>;
