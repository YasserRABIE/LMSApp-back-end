using FluentValidation;

namespace LMS.Application.Auth.Commands.SendOtp;

/// <summary>
/// Validator for SendOtpCommand
/// </summary>
public sealed class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^01[0-9]{9}$")
            .WithMessage("Phone number must be in Egyptian format (01XXXXXXXXX)");
    }
}
