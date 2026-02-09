using LMS.Domain.Common;

namespace LMS.Application.Common;

/// <summary>
/// Represents an error enriched with localized message for use in application layer.
/// Bridge between domain (code-only) and presentation (code + message).
/// </summary>
public sealed record EnrichedError(
    string Code,
    string Message,
    ErrorType Type);
