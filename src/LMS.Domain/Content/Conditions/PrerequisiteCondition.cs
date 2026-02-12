using LMS.Domain.Common;

namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Base class for all prerequisite conditions
/// Uses TPH (Table Per Hierarchy) in EF Core with discriminator
/// </summary>
public abstract class PrerequisiteCondition : Entity<ConditionId>
{
    public PrerequisiteId PrerequisiteId { get; private set; }
    public abstract ConditionType ConditionType { get; }
    public int DisplayOrder { get; private set; }

    protected PrerequisiteCondition() : base()
    {
        PrerequisiteId = null!;
    }

    protected PrerequisiteCondition(ConditionId id, PrerequisiteId prerequisiteId, int displayOrder) : base(id)
    {
        PrerequisiteId = prerequisiteId;
        DisplayOrder = displayOrder;
    }

    public void UpdateDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
}
