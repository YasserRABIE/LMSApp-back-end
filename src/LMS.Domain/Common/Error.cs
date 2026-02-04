namespace LMS.Domain.Common;

/// <summary>
/// Represents an error with a code and message
/// </summary>
public sealed record Error
{
    /// <summary>
    /// Error code (e.g., "USER.NOT_FOUND")
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Error type for categorization
    /// </summary>
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Creates a new error
    /// </summary>
    public static Error Create(string code, string message, ErrorType type = ErrorType.Failure)
        => new(code, message, type);

    /// <summary>
    /// Creates a validation error
    /// </summary>
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    /// <summary>
    /// Creates a not found error
    /// </summary>
    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Creates a conflict error
    /// </summary>
    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized error
    /// </summary>
    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Creates a forbidden error
    /// </summary>
    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// No error (success state)
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}

/// <summary>
/// Error type for categorization and HTTP status code mapping
/// </summary>
public enum ErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    Conflict = 4,
    Unauthorized = 5,
    Forbidden = 6
}
