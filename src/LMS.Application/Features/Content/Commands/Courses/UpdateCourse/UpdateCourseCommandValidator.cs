using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Courses.UpdateCourse;

/// <summary>
/// Validator for UpdateCourseCommand
/// </summary>
public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.SubjectId)
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

        RuleFor(x => x.Visibility)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required))
            .Must(v => v == "Hidden" || v == "Published" || v == "Archived")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.InvalidInput));
    }
}
