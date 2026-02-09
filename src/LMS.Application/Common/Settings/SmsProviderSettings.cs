namespace LMS.Application.Common.Settings;

/// <summary>
/// SMS provider configuration settings for OTP delivery
/// Currently configured for SMS Misr (smsmisr.com)
/// </summary>
public sealed class SmsProviderSettings
{
    /// <summary>
    /// SMS Misr API username
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// SMS Misr API password
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// SMS Misr sender token (from Settings > Sender IDs)
    /// For testing: b611afb996655a94c8e942a823f1421de42bf8335d24ba1f84c437b2ab11ca27
    /// </summary>
    public string SenderToken { get; init; } = string.Empty;

    /// <summary>
    /// SMS Misr template token for OTP message format
    /// Example: 0f9217c9d760c1c0ed47b8afb5425708da7d98729016a8accfc14f9cc8d1ba83 (English)
    /// Example: e83faf6025ec41d0f40256d2812629f5fa9291d05c8322f31eea834302501da8 (Arabic/English)
    /// </summary>
    public string TemplateToken { get; init; } = string.Empty;

    /// <summary>
    /// Environment: 1=Live, 2=Test
    /// </summary>
    public int Environment { get; init; } = 2; // Default to test

    /// <summary>
    /// SMS Misr OTP API URL
    /// </summary>
    public string ApiUrl { get; init; } = "https://smsmisr.com/api/OTP/";

    /// <summary>
    /// Timeout for API requests in seconds
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;
}
