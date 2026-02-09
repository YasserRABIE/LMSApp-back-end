namespace LMS.Application.Common.Settings;

public sealed class FollowUpSettings
{
    public int MaxCallAttempts { get; init; }
    public int CallAttemptIntervalHours { get; init; }
    public int SessionDefaultDurationMinutes { get; init; }
    public int DefaultGroupMaxStudents { get; init; }
    public decimal StageUnlockMinScore { get; init; }
    public bool AutoAssignToLastAssistant { get; init; }
    public bool EnableWaitingList { get; init; }
    public bool EnableAssistantRating { get; init; }
    public bool EnableGroupTransferRequest { get; init; }
}
