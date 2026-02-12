using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Subjects.CreateSubject;

public sealed class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCodes.Subject.NameRequired))
            .MaximumLength(100).WithMessage(ErrorMessages.GetMessage(ErrorCodes.Subject.NameTooLong));

        RuleFor(x => x.Icon)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCodes.Subject.IconRequired));

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCodes.Subject.ColorRequired));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.GetMessage(ErrorCodes.Subject.InvalidDisplayOrder));
    }
}
