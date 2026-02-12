using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.ContentItems.DeleteContentItem;

/// <summary>
/// Validator for DeleteContentItemCommand
/// </summary>
public sealed class DeleteContentItemCommandValidator : AbstractValidator<DeleteContentItemCommand>
{
    public DeleteContentItemCommandValidator()
    {
        RuleFor(x => x.ContentItemId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));
    }
}
