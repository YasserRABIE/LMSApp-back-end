using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Courses.CreateCourse;

/// <summary>
/// Validator for CreateCourseCommand
/// </summary>
public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.TeacherId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.UserIdRequired));

        RuleFor(x => x.SubjectId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.StudyLevelId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.TrackId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.SchoolTypeId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.CourseCategoryId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Course.TitleRequired))
            .MaximumLength(300)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Course.TitleTooLong));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Course.DescriptionRequired));

        RuleFor(x => x.Thumbnail)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Course.ThumbnailRequired));
    }
}
