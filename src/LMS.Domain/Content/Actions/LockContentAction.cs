namespace LMS.Domain.Content.Actions;

/// <summary>
/// Locks content and shows a message to the user
/// </summary>
public sealed class LockContentAction : PrerequisiteAction
{
    public override ActionType ActionType => ActionType.LockContent;
    public string? Message { get; private set; }

    private LockContentAction() : base()
    {
    }

    private LockContentAction(
        ActionId id,
        PrerequisiteId prerequisiteId,
        string? message,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        Message = message;
    }

    public static LockContentAction Create(
        PrerequisiteId prerequisiteId,
        string? message = null,
        int displayOrder = 0)
    {
        return new LockContentAction(
            ActionId.New(),
            prerequisiteId,
            message,
            displayOrder);
    }

    public void UpdateMessage(string? message)
    {
        Message = message;
    }
}
