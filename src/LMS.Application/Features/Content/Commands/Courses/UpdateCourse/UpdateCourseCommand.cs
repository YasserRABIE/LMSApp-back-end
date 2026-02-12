using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.UpdateCourse;

public sealed record UpdateCourseCommand(
    Guid CourseId,
    Guid SubjectId,
    Guid SchoolTypeId,
    Guid CourseCategoryId,
    string Title,
    string Description,
    string Thumbnail,
    string Visibility
) : IRequest<ApiResult>;
