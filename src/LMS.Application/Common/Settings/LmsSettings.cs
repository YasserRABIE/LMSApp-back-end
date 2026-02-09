namespace LMS.Application.Common.Settings;

public sealed class LmsSettings
{
    public AuthenticationSettings Authentication { get; init; } = new();
    public AssessmentSettings Assessment { get; init; } = new();
    public GamificationSettings Gamification { get; init; } = new();
    public StudyPlanSettings StudyPlan { get; init; } = new();
    public FollowUpSettings FollowUp { get; init; } = new();
    public PurchasingSettings Purchasing { get; init; } = new();
    public NotificationSettings Notifications { get; init; } = new();
    public CacheSettings Cache { get; init; } = new();
}
