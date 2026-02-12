using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Stages.DeleteStage;

/// <summary>
/// Validator for DeleteStageCommand
/// </summary>
public sealed class DeleteStageCommandValidator : AbstractValidator<DeleteStageCommand>
{
    public DeleteStageCommandValidator()
    {
        RuleFor(x => x.StageId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));
    }
}
