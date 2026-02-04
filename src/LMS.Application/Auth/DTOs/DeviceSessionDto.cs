namespace LMS.Application.Auth.DTOs;

/// <summary>
/// DTO for device session information
/// </summary>
public sealed record DeviceSessionDto(
    Guid Id,
    string Platform,
    DateTime LastAccessedAtUtc,
    bool IsActive
);
