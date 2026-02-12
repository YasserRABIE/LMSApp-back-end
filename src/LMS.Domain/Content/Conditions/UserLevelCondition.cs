namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires user to be at or above a specific level
/// </summary>
public sealed class UserLevelCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.UserLevel;
    public int MinLevel { get; private set; }

    private UserLevelCondition() : base()
    {
    }

    private UserLevelCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        int minLevel,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        MinLevel = minLevel;
    }

    public static UserLevelCondition Create(
        PrerequisiteId prerequisiteId,
        int minLevel,
        int displayOrder = 0)
    {
        return new UserLevelCondition(
            ConditionId.New(),
            prerequisiteId,
            minLevel,
            displayOrder);
    }
}
