namespace LMS.Domain.Content.Actions;

/// <summary>
/// Redirects user to another content item with a message
/// </summary>
public sealed class RedirectToAction : PrerequisiteAction
{
    public override ActionType ActionType => ActionType.RedirectTo;
    public string TargetContentId { get; private set; }
    public string Message { get; private set; }

    private RedirectToAction() : base()
    {
        TargetContentId = string.Empty;
        Message = string.Empty;
    }

    private RedirectToAction(
        ActionId id,
        PrerequisiteId prerequisiteId,
        string targetContentId,
        string message,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        TargetContentId = targetContentId;
        Message = message;
    }

    public static RedirectToAction Create(
        PrerequisiteId prerequisiteId,
        string targetContentId,
        string message,
        int displayOrder = 0)
    {
        return new RedirectToAction(
            ActionId.New(),
            prerequisiteId,
            targetContentId,
            message,
            displayOrder);
    }

    public void UpdateTarget(string targetContentId, string message)
    {
        TargetContentId = targetContentId;
        Message = message;
    }
}
