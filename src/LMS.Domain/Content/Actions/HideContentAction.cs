namespace LMS.Domain.Content.Actions;

/// <summary>
/// Hides content completely from the user's view
/// </summary>
public sealed class HideContentAction : PrerequisiteAction
{
    public override ActionType ActionType => ActionType.HideContent;

    private HideContentAction() : base()
    {
    }

    private HideContentAction(
        ActionId id,
        PrerequisiteId prerequisiteId,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
    }

    public static HideContentAction Create(
        PrerequisiteId prerequisiteId,
        int displayOrder = 0)
    {
        return new HideContentAction(
            ActionId.New(),
            prerequisiteId,
            displayOrder);
    }
}
