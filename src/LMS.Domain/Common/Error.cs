namespace LMS.Domain.Common;

/// <summary>
/// Represents a domain error with code and type (message-agnostic following Clean Architecture).
/// Messages are added in the Application layer via ErrorMessages.cs for localization support.
/// </summary>
public sealed record Error
{
    /// <summary>
    /// Error code (e.g., "USER.NOT_FOUND")
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Error type for categorization and HTTP status code mapping
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Optional technical description for logging/debugging (NOT for end users)
    /// </summary>
    public string? Description { get; }

    private Error(string code, ErrorType type, string? description = null)
    {
        Code = code;
        Type = type;
        Description = description;
    }

    /// <summary>
    /// Creates a new error with optional technical description
    /// </summary>
    /// <param name="code">Error code from ErrorCodes class</param>
    /// <param name="type">Error type for categorization</param>
    /// <param name="description">Optional technical description for logs (NOT user-facing)</param>
    public static Error Create(string code, ErrorType type = ErrorType.Failure, string? description = null)
        => new(code, type, description);

    /// <summary>
    /// Creates a validation error
    /// </summary>
    /// <param name="code">Error code from ErrorCodes.Validation class</param>
    /// <param name="description">Optional technical description for logs</param>
    public static Error Validation(string code, string? description = null)
        => new(code, ErrorType.Validation, description);

    /// <summary>
    /// Creates a not found error
    /// </summary>
    /// <param name="code">Error code indicating entity not found</param>
    /// <param name="description">Optional technical description for logs</param>
    public static Error NotFound(string code, string? description = null)
        => new(code, ErrorType.NotFound, description);

    /// <summary>
    /// Creates a conflict error (uniqueness violation, state conflict)
    /// </summary>
    /// <param name="code">Error code indicating conflict</param>
    /// <param name="description">Optional technical description for logs</param>
    public static Error Conflict(string code, string? description = null)
        => new(code, ErrorType.Conflict, description);

    /// <summary>
    /// Creates an unauthorized error (authentication failure)
    /// </summary>
    /// <param name="code">Error code indicating authentication failure</param>
    /// <param name="description">Optional technical description for logs</param>
    public static Error Unauthorized(string code, string? description = null)
        => new(code, ErrorType.Unauthorized, description);

    /// <summary>
    /// Creates a forbidden error (authorization failure)
    /// </summary>
    /// <param name="code">Error code indicating insufficient permissions</param>
    /// <param name="description">Optional technical description for logs</param>
    public static Error Forbidden(string code, string? description = null)
        => new(code, ErrorType.Forbidden, description);

    /// <summary>
    /// No error (success state)
    /// </summary>
    public static readonly Error None = new(string.Empty, ErrorType.None);
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
