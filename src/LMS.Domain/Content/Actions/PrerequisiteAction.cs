using LMS.Domain.Common;

namespace LMS.Domain.Content.Actions;

/// <summary>
/// Base class for all prerequisite actions
/// Uses TPH (Table Per Hierarchy) in EF Core with discriminator
/// </summary>
public abstract class PrerequisiteAction : Entity<ActionId>
{
    public PrerequisiteId PrerequisiteId { get; private set; }
    public abstract ActionType ActionType { get; }
    public int DisplayOrder { get; private set; }

    protected PrerequisiteAction() : base()
    {
        PrerequisiteId = null!;
    }

    protected PrerequisiteAction(ActionId id, PrerequisiteId prerequisiteId, int displayOrder) : base(id)
    {
        PrerequisiteId = prerequisiteId;
        DisplayOrder = displayOrder;
    }

    public void UpdateDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
}
