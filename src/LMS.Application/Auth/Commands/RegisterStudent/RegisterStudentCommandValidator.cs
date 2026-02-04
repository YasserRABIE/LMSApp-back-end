using FluentValidation;

namespace LMS.Application.Auth.Commands.RegisterStudent;

/// <summary>
/// Validator for RegisterStudentCommand
/// </summary>
public sealed class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    public RegisterStudentCommandValidator()
    {
        RuleFor(x => x.VerificationToken)
            .NotEmpty()
            .WithMessage("Verification token is required");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MaximumLength(200)
            .WithMessage("Full name cannot exceed 200 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one digit");

        RuleFor(x => x.StudyLevelTrackId)
            .NotEmpty()
            .WithMessage("Study level and track must be selected");

        RuleFor(x => x.SchoolName)
            .MaximumLength(200)
            .WithMessage("School name cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.SchoolName));

        RuleFor(x => x.Governorate)
            .MaximumLength(100)
            .WithMessage("Governorate cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Governorate));
    }
}
