namespace LMS.Application.Common.Settings;

public sealed class StudyPlanSettings
{
    public int DefaultDailyMinutes { get; init; }
    public int MinDailyMinutes { get; init; }
    public int MaxDailyMinutes { get; init; }
    public int MaxReschedulesPerWeek { get; init; }
    public bool AutoAdjustOnMissedDays { get; init; }
    public bool AutoAdjustOnLowScore { get; init; }
    public int LowScoreThresholdPercent { get; init; }
    public int ReviewDayIntervalDays { get; init; }
    public int ReviewQuizQuestionsCount { get; init; }
    public int ModuleDurationDays { get; init; }
    public bool CrampTasksIfLatePurchase { get; init; }
}
