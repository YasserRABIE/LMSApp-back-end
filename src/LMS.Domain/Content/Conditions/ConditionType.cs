namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Types of prerequisite conditions
/// </summary>
public enum ConditionType
{
    ContentCompleted = 1,
    AssessmentScore = 2,
    AssessmentPassed = 3,
    ModuleCompleted = 4,
    StageCompleted = 5,
    TimeElapsed = 6,
    UserHasProduct = 7,
    UserLevel = 8,
    UserTrack = 9,
    DateRange = 10
}
