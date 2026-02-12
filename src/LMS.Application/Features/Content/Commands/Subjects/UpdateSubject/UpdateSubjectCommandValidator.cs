using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.Subjects.UpdateSubject;

public sealed class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subject ID is required");

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
