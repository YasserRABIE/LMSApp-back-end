using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Stages.UpdateStage;

/// <summary>
/// Validator for UpdateStageCommand
/// </summary>
public sealed class UpdateStageCommandValidator : AbstractValidator<UpdateStageCommand>
{
    public UpdateStageCommandValidator()
    {
        RuleFor(x => x.StageId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Stage.TitleRequired))
            .MaximumLength(300)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Stage.TitleTooLong));
    }
}
