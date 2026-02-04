using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Command to refresh access token using refresh token
/// </summary>
public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<ApiResult<AuthTokensDto>>;
