namespace LMS.Application.Features.Auth.DTOs;

/// <summary>
/// DTO for user information
/// </summary>
public sealed record UserInfoDto(
    Guid Id,
    string FullName,
    string Phone,
    string UserType,
    bool IsFirstLogin = false
);
