using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

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
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneRequired))
            .Matches(@"^01[0-9]{9}$")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneInvalidFormat));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordRequired));

        RuleFor(x => x.DeviceFingerprint)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.DeviceFingerprintRequired));

        RuleFor(x => x.Platform)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PlatformRequired))
            .MaximumLength(50)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PlatformMaxLength));
    }
}
