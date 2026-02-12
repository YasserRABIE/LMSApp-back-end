using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.UpdateCourse;

public sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ApiResult>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
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

        // Parse visibility
        if (!Enum.TryParse<Visibility>(request.Visibility, out var visibility))
        {
            return ApiResult.Fail(
                ErrorCodes.Validation.InvalidInput,
                ErrorMessages.GetMessage(ErrorCodes.Validation.InvalidInput),
                HttpStatusCodes.BadRequest);
        }

        // Update course details
        var updateResult = course.UpdateDetails(
            subjectId: SubjectId.From(request.SubjectId),
            title: request.Title,
            description: request.Description,
            thumbnail: request.Thumbnail,
            schoolTypeId: SchoolTypeId.From(request.SchoolTypeId),
            courseCategoryId: CourseCategoryId.From(request.CourseCategoryId));

        if (updateResult.IsFailure)
        {
            var statusCode = updateResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult.Fail(
                updateResult.Error.Code,
                ErrorMessages.GetMessage(updateResult.Error.Code),
                statusCode);
        }

        // Update visibility
        course.ChangeVisibility(visibility);

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.CourseUpdated);
    }
}
