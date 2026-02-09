using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Auth.Commands.RegisterStudent;

/// <summary>
/// Validator for RegisterStudentCommand
/// </summary>
public sealed class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    public RegisterStudentCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.FirstNameRequired))
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.FirstNameMaxLength));

        RuleFor(x => x.SecondName)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.SecondNameRequired))
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.SecondNameMaxLength));

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.LastNameRequired))
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.LastNameMaxLength));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordRequired))
            .MinimumLength(8)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordMinLength))
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordComplexity));

        RuleFor(x => x.StudyLevelTrackId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.StudyLevelTrackRequired));

        RuleFor(x => x.SchoolName)
            .MaximumLength(200)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.SchoolNameMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.SchoolName));

        RuleFor(x => x.Governorate)
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.GovernorateMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Governorate));
    }
}
