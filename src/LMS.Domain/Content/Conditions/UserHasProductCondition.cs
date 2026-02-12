namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires user to own a specific product
/// </summary>
public sealed class UserHasProductCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.UserHasProduct;
    public string RequiredProductId { get; private set; }

    private UserHasProductCondition() : base()
    {
        RequiredProductId = string.Empty;
    }

    private UserHasProductCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        string requiredProductId,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        RequiredProductId = requiredProductId;
    }

    public static UserHasProductCondition Create(
        PrerequisiteId prerequisiteId,
        string requiredProductId,
        int displayOrder = 0)
    {
        return new UserHasProductCondition(
            ConditionId.New(),
            prerequisiteId,
            requiredProductId,
            displayOrder);
    }
}
