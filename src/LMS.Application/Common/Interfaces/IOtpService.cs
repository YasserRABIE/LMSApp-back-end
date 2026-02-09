using LMS.Domain.Common;

namespace LMS.Application.Common.Interfaces;

/// <summary>
/// Service interface for OTP (One-Time Password) operations
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates and stores an OTP for a phone number
    /// </summary>
    /// <param name="phone">The phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The generated OTP code</returns>
    Task<string> GenerateOtpAsync(string phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies an OTP code for a phone number
    /// </summary>
    /// <param name="phone">The phone number</param>
    /// <param name="code">The OTP code to verify</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid, false otherwise</returns>
    Task<bool> VerifyOtpAsync(string phone, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an OTP code via WhatsApp
    /// </summary>
    /// <param name="phone">The phone number</param>
    /// <param name="code">The OTP code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<Result> SendOtpSmsAsync(string phone, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if rate limit has been exceeded for a phone number
    /// </summary>
    /// <param name="phone">The phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if rate limit exceeded, false otherwise</returns>
    Task<bool> IsRateLimitExceededAsync(string phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a verification token after OTP is verified
    /// Used for completing registration
    /// </summary>
    /// <param name="phone">The phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The verification token</returns>
    Task<string> GenerateVerificationTokenAsync(string phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a verification token and returns the associated phone number
    /// </summary>
    /// <param name="token">The verification token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The phone number if valid, null otherwise</returns>
    Task<string?> ValidateVerificationTokenAsync(string token, CancellationToken cancellationToken = default);
}
