using FluentValidation;

namespace LMS.Application.Auth.Commands.LogoutAllDevices;

/// <summary>
/// Validator for LogoutAllDevicesCommand
/// </summary>
public sealed class LogoutAllDevicesCommandValidator : AbstractValidator<LogoutAllDevicesCommand>
{
    public LogoutAllDevicesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
