using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Represents a user's session on a specific device
/// Supports multi-device authentication and management
/// </summary>
public sealed class DeviceSession : Entity<Guid>
{
    /// <summary>
    /// Reference to the user
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Hashed device fingerprint for identification
    /// </summary>
    public string DeviceFingerprintHash { get; private set; }

    /// <summary>
    /// Device platform (iOS, Android, Web, etc.)
    /// </summary>
    public string Platform { get; private set; }

    /// <summary>
    /// Refresh token for this session
    /// </summary>
    public string RefreshToken { get; private set; }

    /// <summary>
    /// When the refresh token expires
    /// </summary>
    public DateTime RefreshTokenExpiresAtUtc { get; private set; }

    /// <summary>
    /// Indicates if the session is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Last IP address used from this session
    /// </summary>
    public string? LastIpAddress { get; private set; }

    /// <summary>
    /// User agent string
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// When the session was last accessed
    /// </summary>
    public DateTime LastAccessedAtUtc { get; private set; }

    /// <summary>
    /// When the session was revoked (if applicable)
    /// </summary>
    public DateTime? RevokedAtUtc { get; private set; }

    // Navigation properties
    // public User User { get; private set; } = null!;

    // EF Core constructor
    private DeviceSession() : base()
    {
        UserId = null!;
        DeviceFingerprintHash = string.Empty;
        Platform = string.Empty;
        RefreshToken = string.Empty;
    }

    private DeviceSession(
        Guid id,
        UserId userId,
        string deviceFingerprintHash,
        string platform,
        string refreshToken,
        DateTime refreshTokenExpiresAtUtc,
        string? ipAddress,
        string? userAgent)
        : base(id)
    {
        UserId = userId;
        DeviceFingerprintHash = deviceFingerprintHash;
        Platform = platform;
        RefreshToken = refreshToken;
        RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;
        IsActive = true;
        LastIpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAtUtc = DateTime.UtcNow;
        LastAccessedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new device session
    /// </summary>
    public static Result<DeviceSession> Create(
        UserId userId,
        string deviceFingerprintHash,
        string platform,
        string refreshToken,
        DateTime refreshTokenExpiresAtUtc,
        string? ipAddress = null,
        string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(deviceFingerprintHash))
        {
            return Result<DeviceSession>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(platform))
        {
            return Result<DeviceSession>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<DeviceSession>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (refreshTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result<DeviceSession>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var session = new DeviceSession(
            Guid.NewGuid(),
            userId,
            deviceFingerprintHash,
            platform,
            refreshToken,
            refreshTokenExpiresAtUtc,
            ipAddress,
            userAgent
        );

        return Result<DeviceSession>.Success(session);
    }

    /// <summary>
    /// Refreshes the session with a new refresh token
    /// </summary>
    public Result Refresh(string newRefreshToken, DateTime newExpiresAtUtc, string? ipAddress = null)
    {
        if (!IsActive)
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.Auth.SessionRevoked)
            );
        }

        if (RefreshTokenExpiresAtUtc < DateTime.UtcNow)
        {
            return Result.Failure(
                Error.Unauthorized(ErrorCodes.Auth.RefreshTokenExpired)
            );
        }

        if (string.IsNullOrWhiteSpace(newRefreshToken))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (newExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        RefreshToken = newRefreshToken;
        RefreshTokenExpiresAtUtc = newExpiresAtUtc;
        LastAccessedAtUtc = DateTime.UtcNow;

        if (ipAddress != null)
        {
            LastIpAddress = ipAddress;
        }

        return Result.Success();
    }

    /// <summary>
    /// Revokes the session
    /// </summary>
    public Result Revoke()
    {
        if (!IsActive)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Auth.SessionRevoked)
            );
        }

        IsActive = false;
        RevokedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Updates last accessed time and IP address
    /// </summary>
    public void UpdateLastAccess(string? ipAddress = null)
    {
        LastAccessedAtUtc = DateTime.UtcNow;

        if (ipAddress != null)
        {
            LastIpAddress = ipAddress;
        }
    }

    /// <summary>
    /// Checks if the refresh token is valid and not expired
    /// </summary>
    public bool IsRefreshTokenValid()
    {
        return IsActive && RefreshTokenExpiresAtUtc > DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the session matches the provided device fingerprint
    /// </summary>
    public bool MatchesFingerprint(string deviceFingerprintHash)
    {
        return DeviceFingerprintHash == deviceFingerprintHash;
    }
}
