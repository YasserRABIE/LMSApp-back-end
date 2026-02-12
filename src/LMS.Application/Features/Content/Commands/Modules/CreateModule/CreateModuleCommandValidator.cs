using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Modules.CreateModule;

/// <summary>
/// Validator for CreateModuleCommand
/// </summary>
public sealed class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Module.TitleRequired))
            .MaximumLength(300)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Module.TitleTooLong));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Module.InvalidDisplayOrder));

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Module.InvalidEstimatedHours));
    }
}
