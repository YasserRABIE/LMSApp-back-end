using LMS.Application.Common.Settings;
using LMS.Domain.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LMS.Infrastructure.ExternalServices.SmsMisr;

/// <summary>
/// SMS Misr OTP service for sending verification codes via SMS
/// Documentation: https://smsmisr.com/
/// </summary>
public sealed class SmsMisrOtpService
{
    private readonly HttpClient _httpClient;
    private readonly SmsProviderSettings _settings;
    private readonly ILogger<SmsMisrOtpService> _logger;
    private readonly bool _isConfigured;

    public SmsMisrOtpService(
        HttpClient httpClient,
        IOptions<SmsProviderSettings> settings,
        ILogger<SmsMisrOtpService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        // Check if SMS provider is properly configured
        _isConfigured = !string.IsNullOrWhiteSpace(_settings.Username) &&
                       !string.IsNullOrWhiteSpace(_settings.Password) &&
                       !string.IsNullOrWhiteSpace(_settings.SenderToken) &&
                       !string.IsNullOrWhiteSpace(_settings.TemplateToken);

        if (_isConfigured)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
            _logger.LogInformation("SMS Misr OTP service initialized successfully");
        }
        else
        {
            _logger.LogWarning("SMS Misr OTP service not configured. Username, Password, SenderToken, or TemplateToken is missing.");
        }
    }

    /// <summary>
    /// Sends OTP via SMS using SMS Misr API
    /// </summary>
    /// <param name="phoneNumber">Phone number in Egyptian format (01...)</param>
    /// <param name="otpCode">OTP code (max 10 characters)</param>
    /// <param name="expiryMinutes">OTP expiry time in minutes (not used by SMS Misr API)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> SendSmsOtpAsync(
        string phoneNumber,
        string otpCode,
        int expiryMinutes,
        CancellationToken cancellationToken = default)
    {
        // If not configured, fall back to console logging for development
        if (!_isConfigured)
        {
            _logger.LogWarning(
                "SMS Misr not configured. Would send OTP {OtpCode} to {Phone} via SMS",
                otpCode,
                phoneNumber);

            Console.WriteLine($"[SMS Misr - DEV MODE] OTP {otpCode} for {phoneNumber}");
            await Task.CompletedTask;
            return Result.Success();
        }

        try
        {
            // Format phone number for SMS Misr (01... → 2011...)
            var formattedPhone = FormatPhoneNumber(phoneNumber);

            _logger.LogInformation(
                "Sending SMS OTP to {Phone} via SMS Misr (Environment: {Environment})",
                formattedPhone,
                _settings.Environment == 1 ? "Live" : "Test");

            // Build query parameters for SMS Misr OTP API (POST only as per official docs)
            var queryParams = new Dictionary<string, string>
            {
                { "environment", _settings.Environment.ToString() },
                { "username", _settings.Username },
                { "password", _settings.Password },
                { "sender", _settings.SenderToken },
                { "mobile", formattedPhone },
                { "template", _settings.TemplateToken },
                { "otp", otpCode }
            };

            // Build URL with query string (POST request with query parameters)
            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var requestUrl = $"{_settings.ApiUrl}?{queryString}";

            // Make POST request to SMS Misr API (POST only as stated in docs)
            var response = await _httpClient.PostAsync(requestUrl, null, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                
                // Parse SMS Misr response
                var smsResponse = JsonSerializer.Deserialize<SmsMisrResponse>(responseContent);

                if (smsResponse?.Code == "4901")
                {
                    _logger.LogInformation(
                        "SMS OTP sent successfully to {Phone}. SMSID: {SmsId}, Cost: {Cost}",
                        formattedPhone,
                        smsResponse.SMSID,
                        smsResponse.Cost);

                    return Result.Success();
                }
                else
                {
                    // Map SMS Misr error codes to domain errors
                    var errorResult = MapSmsMisrError(smsResponse?.Code ?? "unknown", responseContent);
                    _logger.LogError(
                        "SMS Misr API returned error code {ErrorCode}",
                        smsResponse?.Code);

                    return errorResult;
                }
            }
            else
            {
                _logger.LogError(
                    "SMS Misr API HTTP error. StatusCode: {StatusCode}, Response: {Response}",
                    response.StatusCode,
                    responseContent);

                return Result.Failure(
                    Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, $"SMS API returned {response.StatusCode}"));
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP error sending SMS OTP to {Phone}",
                phoneNumber);

            return Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, $"Failed to send SMS OTP: {ex.Message}"));
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to parse SMS Misr response");

            return Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, "Failed to parse SMS provider response"));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error sending SMS OTP to {Phone}",
                phoneNumber);

            return Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, "An unexpected error occurred while sending SMS"));
        }
    }

    /// <summary>
    /// Formats phone number for SMS Misr API
    /// Converts: 01012345678 → 201012345678 (12 digits)
    /// </summary>
    private string FormatPhoneNumber(string phoneNumber)
    {
        // Remove any spaces, dashes, or special characters
        var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

        // If it starts with +20, remove the + (already has country code)
        if (phoneNumber.StartsWith("+20"))
        {
            return cleaned; // Keep 20...
        }

        // If it starts with 20 (without +) and has 12 digits, keep as is
        if (cleaned.StartsWith("20") && cleaned.Length == 12)
        {
            return cleaned;
        }

        // If it's in 01... format (11 digits), remove leading 0 and add country code 20
        if (cleaned.StartsWith("01") && cleaned.Length == 11)
        {
            return $"20{cleaned.Substring(1)}"; // 01012345678 → 201012345678 (remove 0, add 20)
        }

        // If it starts with 1 (without 0) and has 10 digits, add 20
        if (cleaned.StartsWith("1") && cleaned.Length == 10)
        {
            return $"20{cleaned}"; // 1012345678 → 201012345678
        }

        // Default: return cleaned number
        return cleaned;
    }

    /// <summary>
    /// Maps SMS Misr error codes to domain error results
    /// </summary>
    private Result MapSmsMisrError(string errorCode, string responseContent)
    {
        return errorCode switch
        {
            "4903" => Result.Failure(
                Error.Create(ErrorCodes.Sms.InvalidCredentials, ErrorType.Failure, "Invalid SMS provider username or password")),

            "4904" => Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, "Invalid SMS sender token")),

            "4905" => Result.Failure(
                Error.Create(ErrorCodes.Sms.InvalidRecipient, ErrorType.Validation, "Invalid mobile number format")),

            "4906" => Result.Failure(
                Error.Create(ErrorCodes.Sms.InsufficientBalance, ErrorType.Failure, "Insufficient SMS credit balance")),

            "4907" => Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, "SMS server is updating, please try again later")),

            "4908" => Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Validation, "Invalid OTP format (max 10 characters)")),

            "4909" => Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, "Invalid SMS template token")),

            "4912" => Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Validation, "Invalid environment setting (must be 1 or 2)")),

            _ => Result.Failure(
                Error.Create(ErrorCodes.Sms.SendFailed, ErrorType.Failure, $"SMS sending failed with code: {errorCode}. Response: {responseContent}"))
        };
    }

    /// <summary>
    /// SMS Misr API response model
    /// </summary>
    private class SmsMisrResponse
    {
        public string? Code { get; set; }
        public string? SMSID { get; set; }
        public string? Cost { get; set; }
    }
}
