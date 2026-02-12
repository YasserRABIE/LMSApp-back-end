namespace LMS.Domain.Content.Actions;

/// <summary>
/// Allows access but disables rewards (XP, purchasing points)
/// </summary>
public sealed class DisableRewardsAction : PrerequisiteAction
{
    public override ActionType ActionType => ActionType.DisableRewards;
    public bool AffectXp { get; private set; }
    public bool AffectPurchasingPoints { get; private set; }

    private DisableRewardsAction() : base()
    {
    }

    private DisableRewardsAction(
        ActionId id,
        PrerequisiteId prerequisiteId,
        bool affectXp,
        bool affectPurchasingPoints,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        AffectXp = affectXp;
        AffectPurchasingPoints = affectPurchasingPoints;
    }

    public static DisableRewardsAction Create(
        PrerequisiteId prerequisiteId,
        bool affectXp = true,
        bool affectPurchasingPoints = true,
        int displayOrder = 0)
    {
        return new DisableRewardsAction(
            ActionId.New(),
            prerequisiteId,
            affectXp,
            affectPurchasingPoints,
            displayOrder);
    }
}
