namespace LMS.Application.Features.Auth.DTOs;

/// <summary>
/// DTO for verification token after OTP verification
/// </summary>
public sealed record VerificationTokenDto(
    string Token,
    DateTime ExpiresAt
);
