using LMS.Application.Common;
using LMS.Application.Features.Auth.DTOs;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command to refresh access token using refresh token
/// </summary>
public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<ApiResult<AuthTokensDto>>;
