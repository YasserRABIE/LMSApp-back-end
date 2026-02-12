using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command to logout from a specific device session
/// </summary>
public sealed record LogoutCommand(
    Guid SessionId
) : IRequest<ApiResult>;
