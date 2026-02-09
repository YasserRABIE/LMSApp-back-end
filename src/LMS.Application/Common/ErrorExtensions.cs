using LMS.Domain.Common;

namespace LMS.Application.Common;

/// <summary>
/// Extension methods for enriching domain errors with localized messages.
/// Bridge between domain layer (code-only) and application layer (code + message).
/// </summary>
public static class ErrorExtensions
{
    /// <summary>
    /// Converts domain Error to EnrichedError with localized message.
    /// This is the bridge between domain (code-only) and presentation (code + message).
    /// </summary>
    /// <param name="error">Domain error (code + type only)</param>
    /// <param name="language">Language code: "ar" (Arabic, default) or "en" (English)</param>
    /// <param name="formatArgs">Optional parameters for message formatting (e.g., cooldown time)</param>
    /// <returns>Enriched error with localized user-facing message</returns>
    /// <example>
    /// // Domain returns error code only
    /// var result = Phone.Create(invalidPhone);
    /// if (result.IsFailure)
    /// {
    ///     // Application enriches with localized message
    ///     var enrichedError = result.Error!.WithMessage();
    ///     // enrichedError.Message = "صيغة رقم الهاتف غير صحيحة. الصيغة المتوقعة: 01XXXXXXXXX"
    /// }
    /// </example>
    public static EnrichedError WithMessage(
        this Error error,
        string language = "ar",
        params object[] formatArgs)
    {
        var message = ErrorMessages.GetMessage(error.Code, language, formatArgs);
        return new EnrichedError(error.Code, message, error.Type);
    }
}
