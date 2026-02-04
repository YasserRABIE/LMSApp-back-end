using FluentValidation;

namespace LMS.Application.Auth.Commands.Login;

/// <summary>
/// Validator for LoginCommand
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^01[0-9]{9}$")
            .WithMessage("Phone number must be in Egyptian format (01XXXXXXXXX)");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");

        RuleFor(x => x.DeviceFingerprint)
            .NotEmpty()
            .WithMessage("Device fingerprint is required");

        RuleFor(x => x.Platform)
            .NotEmpty()
            .WithMessage("Platform is required")
            .MaximumLength(50)
            .WithMessage("Platform cannot exceed 50 characters");
    }
}
