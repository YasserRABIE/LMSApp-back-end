namespace LMS.Application.Features.Auth.DTOs;

/// <summary>
/// DTO for login response containing tokens and user info
/// </summary>
public sealed record LoginResponseDto(
    AuthTokensDto Tokens,
    UserInfoDto User
);
