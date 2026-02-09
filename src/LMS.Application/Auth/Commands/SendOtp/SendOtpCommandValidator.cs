using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

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
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneRequired))
            .Matches(@"^01[0-9]{9}$")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneInvalidFormat));
    }
}
