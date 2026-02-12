namespace LMS.Domain.Content.Actions;

/// <summary>
/// Reduces rewards by a multiplier (e.g., 0.5 = 50% rewards)
/// </summary>
public sealed class ReduceRewardsAction : PrerequisiteAction
{
    public override ActionType ActionType => ActionType.ReduceRewards;
    public decimal XpMultiplier { get; private set; }
    public decimal PurchasingPointsMultiplier { get; private set; }

    private ReduceRewardsAction() : base()
    {
    }

    private ReduceRewardsAction(
        ActionId id,
        PrerequisiteId prerequisiteId,
        decimal xpMultiplier,
        decimal purchasingPointsMultiplier,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        XpMultiplier = xpMultiplier;
        PurchasingPointsMultiplier = purchasingPointsMultiplier;
    }

    public static ReduceRewardsAction Create(
        PrerequisiteId prerequisiteId,
        decimal xpMultiplier = 0.5m,
        decimal purchasingPointsMultiplier = 0.5m,
        int displayOrder = 0)
    {
        return new ReduceRewardsAction(
            ActionId.New(),
            prerequisiteId,
            xpMultiplier,
            purchasingPointsMultiplier,
            displayOrder);
    }
}
