namespace LMS.Domain.Content.Actions;

/// <summary>
/// Shows a warning but allows access
/// </summary>
public sealed class ShowWarningAction : PrerequisiteAction
{
    public override ActionType ActionType => ActionType.ShowWarning;
    public string Message { get; private set; }

    private ShowWarningAction() : base()
    {
        Message = string.Empty;
    }

    private ShowWarningAction(
        ActionId id,
        PrerequisiteId prerequisiteId,
        string message,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        Message = message;
    }

    public static ShowWarningAction Create(
        PrerequisiteId prerequisiteId,
        string message,
        int displayOrder = 0)
    {
        return new ShowWarningAction(
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
