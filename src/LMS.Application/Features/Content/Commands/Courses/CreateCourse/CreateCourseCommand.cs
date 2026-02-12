using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.CreateCourse;

public sealed record CreateCourseCommand(
    Guid TeacherId,
    Guid SubjectId,
    Guid StudyLevelId,
    Guid TrackId,
    Guid SchoolTypeId,
    Guid CourseCategoryId,
    string Title,
    string Description,
    string Thumbnail
) : IRequest<ApiResult<Guid>>;
