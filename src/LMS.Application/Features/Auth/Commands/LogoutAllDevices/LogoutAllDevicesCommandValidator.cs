using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Auth.Commands.LogoutAllDevices;

/// <summary>
/// Validator for LogoutAllDevicesCommand
/// </summary>
public sealed class LogoutAllDevicesCommandValidator : AbstractValidator<LogoutAllDevicesCommand>
{
    public LogoutAllDevicesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.UserIdRequired));
    }
}
