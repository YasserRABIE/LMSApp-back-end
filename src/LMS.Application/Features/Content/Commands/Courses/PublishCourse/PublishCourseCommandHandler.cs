using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.PublishCourse;

public sealed class PublishCourseCommandHandler : IRequestHandler<PublishCourseCommand, ApiResult>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishCourseCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(PublishCourseCommand request, CancellationToken cancellationToken)
    {
        // Get the course
        var course = await _courseRepository.GetByIdAsync(CourseId.From(request.CourseId), cancellationToken);
        if (course == null)
        {
            return ApiResult.Fail(
                ErrorCodes.Course.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Course.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Publish the course
        var publishResult = course.Publish();
        if (publishResult.IsFailure)
        {
            var statusCode = publishResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult.Fail(
                publishResult.Error.Code,
                ErrorMessages.GetMessage(publishResult.Error.Code),
                statusCode);
        }

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.CoursePublished);
    }
}
