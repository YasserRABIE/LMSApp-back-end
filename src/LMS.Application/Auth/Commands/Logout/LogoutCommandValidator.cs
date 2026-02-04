using FluentValidation;

namespace LMS.Application.Auth.Commands.Logout;

/// <summary>
/// Validator for LogoutCommand
/// </summary>
public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("Session ID is required");
    }
}
