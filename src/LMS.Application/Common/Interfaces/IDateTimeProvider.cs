namespace LMS.Application.Common.Interfaces;

/// <summary>
/// Service interface for providing current date and time
/// Allows for testing with controlled time
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Gets the current local date and time
    /// </summary>
    DateTime Now { get; }
}
