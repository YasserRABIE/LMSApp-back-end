namespace LMS.Domain.FollowUp;

/// <summary>
/// Status of a student on the waiting list
/// </summary>
public enum WaitingListStatus : byte
{
    Waiting = 0,
    Assigned = 1,
    Cancelled = 2
}

/// <summary>
/// Status of a student's follow-up enrollment
/// </summary>
public enum FollowUpEnrollmentStatus : byte
{
    Active = 0,
    Paused = 1,
    Cancelled = 2,
    Completed = 3
}

/// <summary>
/// How a follow-up session was triggered
/// </summary>
public enum TriggerType : byte
{
    AfterStage = 1,
    Scheduled = 2,
    Manual = 3
}

/// <summary>
/// Status of a follow-up call/session
/// </summary>
public enum CallStatus : byte
{
    Pending = 0,
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    NoAnswer = 4,
    Rescheduled = 5
}

/// <summary>
/// Student status from assistant evaluation
/// </summary>
public enum StudentStatus : byte
{
    Excellent = 1,
    Good = 2,
    NeedsImprovement = 3,
    AtRisk = 4
}

/// <summary>
/// Recommended action from assistant evaluation
/// </summary>
public enum RecommendedAction : byte
{
    Continue = 1,
    AddReview = 2,
    Warning = 3,
    ContactParent = 4
}
