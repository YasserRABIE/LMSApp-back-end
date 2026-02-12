using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.DeleteCourse;

public sealed class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, ApiResult>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
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

        // Soft delete by deactivating
        course.Deactivate();

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.CourseDeleted);
    }
}
