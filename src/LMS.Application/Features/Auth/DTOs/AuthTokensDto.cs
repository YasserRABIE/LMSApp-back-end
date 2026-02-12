namespace LMS.Application.Features.Auth.DTOs;

/// <summary>
/// DTO for authentication tokens
/// </summary>
public sealed record AuthTokensDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
