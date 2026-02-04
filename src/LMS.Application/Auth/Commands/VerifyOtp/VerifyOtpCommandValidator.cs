using FluentValidation;

namespace LMS.Application.Auth.Commands.VerifyOtp;

/// <summary>
/// Validator for VerifyOtpCommand
/// </summary>
public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^01[0-9]{9}$")
            .WithMessage("Phone number must be in Egyptian format (01XXXXXXXXX)");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("OTP code is required")
            .Length(6)
            .WithMessage("OTP code must be 6 digits")
            .Matches(@"^\d{6}$")
            .WithMessage("OTP code must contain only digits");
    }
}
