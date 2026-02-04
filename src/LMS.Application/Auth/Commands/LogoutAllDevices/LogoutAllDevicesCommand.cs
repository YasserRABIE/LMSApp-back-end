using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.LogoutAllDevices;

/// <summary>
/// Command to logout from all device sessions
/// </summary>
public sealed record LogoutAllDevicesCommand(
    Guid UserId
) : IRequest<ApiResult>;
