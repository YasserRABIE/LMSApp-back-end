namespace LMS.Application.Common.Settings;

public sealed class GamificationSettings
{
    public int StreakResetHourUtc { get; init; }
    public int DefaultXpPerLecture { get; init; }
    public int DefaultXpPerQuiz { get; init; }
    public int DefaultXpPerAssignment { get; init; }
    public int DefaultPointsPerLecture { get; init; }
    public int DefaultPointsPerQuiz { get; init; }
    public int DefaultPointsPerAssignment { get; init; }
    public int StreakBonusXpPerDay { get; init; }
    public int StreakBonusPointsPerDay { get; init; }
    public int LeaderboardUpdateIntervalMinutes { get; init; }
    public bool LeaderboardTop100ShowRank { get; init; }
    public int LeaderboardShowPercentageAfterRank { get; init; }
    public List<StreakFreezeRule> StreakFreezeRules { get; init; } = [];
}

public sealed class StreakFreezeRule
{
    public int MinStreak { get; init; }
    public int MaxStreak { get; init; }
    public int Freezes { get; init; }
}
