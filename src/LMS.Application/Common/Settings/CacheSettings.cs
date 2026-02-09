namespace LMS.Application.Common.Settings;

public sealed class CacheSettings
{
    public int DefaultExpiryMinutes { get; init; }
    public int LeaderboardExpiryMinutes { get; init; }
    public int StudentGamificationExpiryMinutes { get; init; }
    public int CourseListExpiryMinutes { get; init; }
}
