using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user and create a session
/// </summary>
public sealed record LoginCommand(
    string Phone,
    string Password,
    string DeviceFingerprint,
    string Platform
) : IRequest<ApiResult<LoginResponseDto>>;
