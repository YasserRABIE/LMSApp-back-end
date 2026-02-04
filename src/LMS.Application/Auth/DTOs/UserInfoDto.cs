namespace LMS.Application.Auth.DTOs;

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
