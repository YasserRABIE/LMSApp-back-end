using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.PublishCourse;

public sealed record PublishCourseCommand(
    Guid CourseId
) : IRequest<ApiResult>;
