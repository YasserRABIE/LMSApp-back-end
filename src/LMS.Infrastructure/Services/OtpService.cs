using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Settings;
using LMS.Domain.Common;
using LMS.Infrastructure.Caching;
using LMS.Infrastructure.ExternalServices.SmsMisr;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Services;

/// <summary>
/// OTP service implementation using Redis for storage and SMS for delivery
/// </summary>
public sealed class OtpService : IOtpService
{
    private readonly RedisCacheService _cache;
    private readonly AuthenticationSettings _authSettings;
    private readonly SmsMisrOtpService? _smsService;
    private const int VerificationTokenExpiryMinutes = 15;

    // Redis key patterns
    private const string OtpKeyPattern = "otp:{0}"; // otp:phone
    private const string RateLimitKeyPattern = "otp:ratelimit:{0}"; // otp:ratelimit:phone
    private const string VerificationTokenKeyPattern = "otp:verification:{0}"; // otp:verification:token

    public OtpService(
        RedisCacheService cache,
        IOptions<AuthenticationSettings> authSettings,
        SmsMisrOtpService? smsService = null)
    {
        _cache = cache;
        _authSettings = authSettings.Value;
        _smsService = smsService;
    }

    public async Task<string> GenerateOtpAsync(string phone, CancellationToken cancellationToken = default)
    {
        // Generate OTP with configured length
        var random = new Random();
        var minValue = (int)Math.Pow(10, _authSettings.OtpLength - 1);
        var maxValue = (int)Math.Pow(10, _authSettings.OtpLength) - 1;
        var otp = random.Next(minValue, maxValue).ToString();

        // Store in Redis with expiry from settings
        var key = string.Format(OtpKeyPattern, phone);
        await _cache.SetStringAsync(
            key,
            otp,
            TimeSpan.FromMinutes(_authSettings.OtpExpiryMinutes),
            cancellationToken);

        // Increment rate limit counter with cooldown from settings
        var rateLimitKey = string.Format(RateLimitKeyPattern, phone);
        await _cache.IncrementAsync(
            rateLimitKey,
            TimeSpan.FromMinutes(_authSettings.OtpCooldownMinutes),
            cancellationToken);

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(string phone, string code, CancellationToken cancellationToken = default)
    {
        // Development bypass: accept any code (or use fixed "000000")
        if (_authSettings.BypassOtpInDevelopment)
        {
            Console.WriteLine($"[OTP Service - DEV MODE] Bypassing OTP verification for {phone}. Code: {code} accepted.");
            return true;
        }

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
        // If SMS service is available, use it
        if (_smsService != null)
        {
            return await _smsService.SendSmsOtpAsync(
                phone,
                code,
                _authSettings.OtpExpiryMinutes,
                cancellationToken);
        }

        // Fallback to console logging for development/testing
        Console.WriteLine($"[OTP Service] Sending OTP {code} to {phone}");

        return Result.Success();
    }

    public async Task<bool> IsRateLimitExceededAsync(string phone, CancellationToken cancellationToken = default)
    {
        var rateLimitKey = string.Format(RateLimitKeyPattern, phone);
        var attempts = await _cache.GetAsync<int>(rateLimitKey, cancellationToken);

        return attempts >= _authSettings.OtpMaxAttempts;
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
