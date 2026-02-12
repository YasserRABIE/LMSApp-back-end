using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Tags.CreateTag;

public sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCodes.Tag.NameRequired))
            .MaximumLength(50).WithMessage(ErrorMessages.GetMessage(ErrorCodes.Tag.NameTooLong));
    }
}
