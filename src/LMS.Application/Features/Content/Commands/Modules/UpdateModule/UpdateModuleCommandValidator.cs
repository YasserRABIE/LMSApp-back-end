using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Modules.UpdateModule;

/// <summary>
/// Validator for UpdateModuleCommand
/// </summary>
public sealed class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
{
    public UpdateModuleCommandValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Module.TitleRequired))
            .MaximumLength(300)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Module.TitleTooLong));

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Module.InvalidEstimatedHours));
    }
}
