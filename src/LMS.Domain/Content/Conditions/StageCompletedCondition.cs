namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires a specific stage to be completed
/// </summary>
public sealed class StageCompletedCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.StageCompleted;
    public string RequiredStageId { get; private set; }

    private StageCompletedCondition() : base()
    {
        RequiredStageId = string.Empty;
    }

    private StageCompletedCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        string requiredStageId,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        RequiredStageId = requiredStageId;
    }

    public static StageCompletedCondition Create(
        PrerequisiteId prerequisiteId,
        string requiredStageId,
        int displayOrder = 0)
    {
        return new StageCompletedCondition(
            ConditionId.New(),
            prerequisiteId,
            requiredStageId,
            displayOrder);
    }
}
