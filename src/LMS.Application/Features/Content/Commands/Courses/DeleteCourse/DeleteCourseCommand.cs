using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.DeleteCourse;

public sealed record DeleteCourseCommand(
    Guid CourseId
) : IRequest<ApiResult>;
