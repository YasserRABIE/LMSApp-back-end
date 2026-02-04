using LMS.Application.Common.Interfaces;

namespace LMS.Infrastructure.Services;

/// <summary>
/// Implementation of IDateTimeProvider for testable datetime operations
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}
