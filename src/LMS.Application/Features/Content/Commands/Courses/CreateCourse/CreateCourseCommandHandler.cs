using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using LMS.Domain.Users;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Courses.CreateCourse;

public sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ApiResult<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        // Create course using domain factory method
        var courseResult = Course.Create(
            teacherId: UserId.From(request.TeacherId),
            subjectId: SubjectId.From(request.SubjectId),
            studyLevelId: request.StudyLevelId,
            trackId: request.TrackId,
            schoolTypeId: SchoolTypeId.From(request.SchoolTypeId),
            courseCategoryId: CourseCategoryId.From(request.CourseCategoryId),
            title: request.Title,
            description: request.Description,
            thumbnail: request.Thumbnail);

        if (courseResult.IsFailure)
        {
            var statusCode = courseResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult<Guid>.Fail(
                courseResult.Error.Code,
                ErrorMessages.GetMessage(courseResult.Error.Code),
                statusCode);
        }

        // Persist the course
        await _courseRepository.AddAsync(courseResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Guid>.Ok(
            courseResult.Value.Id.Value,
            SuccessMessages.CourseCreated,
            HttpStatusCodes.Created);
    }
}
