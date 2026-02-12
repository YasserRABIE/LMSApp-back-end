namespace LMS.Domain.Content.Actions;

/// <summary>
/// Requires user confirmation before allowing access
/// </summary>
public sealed class RequireConfirmationAction : PrerequisiteAction
{
    public override ActionType ActionType => ActionType.RequireConfirmation;
    public string Message { get; private set; }

    private RequireConfirmationAction() : base()
    {
        Message = string.Empty;
    }

    private RequireConfirmationAction(
        ActionId id,
        PrerequisiteId prerequisiteId,
        string message,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        Message = message;
    }

    public static RequireConfirmationAction Create(
        PrerequisiteId prerequisiteId,
        string message,
        int displayOrder = 0)
    {
        return new RequireConfirmationAction(
            ActionId.New(),
            prerequisiteId,
            message,
            displayOrder);
    }

    public void UpdateMessage(string message)
    {
        Message = message;
    }
}
