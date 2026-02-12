namespace LMS.Domain.Content.Actions;

/// <summary>
/// Types of actions taken when prerequisites are not met
/// </summary>
public enum ActionType
{
    LockContent = 1,
    HideContent = 2,
    DisableRewards = 3,
    ReduceRewards = 4,
    ShowWarning = 5,
    RequireConfirmation = 6,
    RedirectTo = 7
}
