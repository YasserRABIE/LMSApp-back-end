using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Tags.UpdateTag;

public sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Tag ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCodes.Tag.NameRequired))
            .MaximumLength(50).WithMessage(ErrorMessages.GetMessage(ErrorCodes.Tag.NameTooLong));
    }
}
