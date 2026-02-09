using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

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
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneRequired))
            .Matches(@"^01[0-9]{9}$")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneInvalidFormat));

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.OtpCodeRequired))
            .Length(6)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.OtpCodeLength))
            .Matches(@"^\d{6}$")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.OtpCodeDigitsOnly));
    }
}
