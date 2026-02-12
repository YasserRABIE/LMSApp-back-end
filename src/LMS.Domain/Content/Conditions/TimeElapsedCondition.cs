namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires a specific amount of time to pass
/// </summary>
public sealed class TimeElapsedCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.TimeElapsed;
    public int DurationHours { get; private set; }
    public TimeReferencePoint ReferencePoint { get; private set; }

    private TimeElapsedCondition() : base()
    {
    }

    private TimeElapsedCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        int durationHours,
        TimeReferencePoint referencePoint,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        DurationHours = durationHours;
        ReferencePoint = referencePoint;
    }

    public static TimeElapsedCondition Create(
        PrerequisiteId prerequisiteId,
        int durationHours,
        TimeReferencePoint referencePoint = TimeReferencePoint.Enrollment,
        int displayOrder = 0)
    {
        return new TimeElapsedCondition(
            ConditionId.New(),
            prerequisiteId,
            durationHours,
            referencePoint,
            displayOrder);
    }
}

public enum TimeReferencePoint
{
    Enrollment = 1,
    PreviousContentCompletion = 2
}
