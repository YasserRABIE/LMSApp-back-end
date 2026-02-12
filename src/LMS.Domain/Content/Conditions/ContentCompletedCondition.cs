namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires a specific content item to be completed
/// </summary>
public sealed class ContentCompletedCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.ContentCompleted;
    public string RequiredContentId { get; private set; }

    private ContentCompletedCondition() : base()
    {
        RequiredContentId = string.Empty;
    }

    private ContentCompletedCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        string requiredContentId,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        RequiredContentId = requiredContentId;
    }

    public static ContentCompletedCondition Create(
        PrerequisiteId prerequisiteId,
        string requiredContentId,
        int displayOrder = 0)
    {
        return new ContentCompletedCondition(
            ConditionId.New(),
            prerequisiteId,
            requiredContentId,
            displayOrder);
    }
}
