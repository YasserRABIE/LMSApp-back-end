using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Infrastructure.Caching;

namespace LMS.Infrastructure.Services;

/// <summary>
/// OTP service implementation using Redis for storage
/// </summary>
public sealed class OtpService : IOtpService
{
    private readonly RedisCacheService _cache;
    private const int OtpLength = 6;
    private const int OtpExpiryMinutes = 10;
    private const int VerificationTokenExpiryMinutes = 15;
    private const int RateLimitWindowMinutes = 5;
    private const int MaxOtpAttemptsPerWindow = 3;

    // Redis key patterns
    private const string OtpKeyPattern = "otp:{0}"; // otp:phone
    private const string RateLimitKeyPattern = "otp:ratelimit:{0}"; // otp:ratelimit:phone
    private const string VerificationTokenKeyPattern = "otp:verification:{0}"; // otp:verification:token

    public OtpService(RedisCacheService cache)
    {
        _cache = cache;
    }

    public async Task<string> GenerateOtpAsync(string phone, CancellationToken cancellationToken = default)
    {
        // Generate 6-digit OTP
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();

        // Store in Redis with expiry
        var key = string.Format(OtpKeyPattern, phone);
        await _cache.SetStringAsync(
            key,
            otp,
            TimeSpan.FromMinutes(OtpExpiryMinutes),
            cancellationToken);

        // Increment rate limit counter
        var rateLimitKey = string.Format(RateLimitKeyPattern, phone);
        await _cache.IncrementAsync(
            rateLimitKey,
            TimeSpan.FromMinutes(RateLimitWindowMinutes),
            cancellationToken);

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(string phone, string code, CancellationToken cancellationToken = default)
    {
        var key = string.Format(OtpKeyPattern, phone);
        var storedOtp = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(storedOtp))
            return false;

        var isValid = storedOtp == code;

        // Remove OTP after verification attempt (whether successful or not)
        if (isValid)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        return isValid;
    }

    public async Task<Result> SendOtpSmsAsync(string phone, string code, CancellationToken cancellationToken = default)
    {
        // TODO: Integrate with SMS provider (Vonage/Twilio)
        // For now, just log the OTP (in production, this should send actual SMS)

        // Development mode - just return success
        // In production, call actual SMS API:
        // - Vonage SMS API
        // - Twilio SMS API
        // - etc.

        await Task.CompletedTask;

        // For development/testing - log the OTP
        Console.WriteLine($"[OTP Service] Sending OTP {code} to {phone}");

        return Result.Success();
    }

    public async Task<bool> IsRateLimitExceededAsync(string phone, CancellationToken cancellationToken = default)
    {
        var rateLimitKey = string.Format(RateLimitKeyPattern, phone);
        var attempts = await _cache.GetAsync<int>(rateLimitKey, cancellationToken);

        return attempts >= MaxOtpAttemptsPerWindow;
    }

    public async Task<string> GenerateVerificationTokenAsync(string phone, CancellationToken cancellationToken = default)
    {
        // Generate secure random token
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        // Store phone number with token as key
        var key = string.Format(VerificationTokenKeyPattern, token);
        await _cache.SetStringAsync(
            key,
            phone,
            TimeSpan.FromMinutes(VerificationTokenExpiryMinutes),
            cancellationToken);

        return token;
    }

    public async Task<string?> ValidateVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var key = string.Format(VerificationTokenKeyPattern, token);
        var phone = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(phone))
            return null;

        // Remove token after validation (single use)
        await _cache.RemoveAsync(key, cancellationToken);

        return phone;
    }
}
