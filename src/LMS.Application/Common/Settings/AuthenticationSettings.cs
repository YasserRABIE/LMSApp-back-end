namespace LMS.Application.Common.Settings;

public sealed class AuthenticationSettings
{
    public int OtpExpiryMinutes { get; init; }
    public int OtpMaxAttempts { get; init; }
    public int OtpCooldownMinutes { get; init; }
    public int OtpLength { get; init; }
    public int DeviceLimit { get; init; }
    public int JwtExpiryMinutes { get; init; }
    public int RefreshTokenExpiryDays { get; init; }
    public int PasswordMinLength { get; init; }
    public bool RequirePhoneVerification { get; init; }

    /// <summary>
    /// When true, any OTP code will be accepted (for development only)
    /// Use fixed code "000000" for easy testing
    /// </summary>
    public bool BypassOtpInDevelopment { get; init; }
}
