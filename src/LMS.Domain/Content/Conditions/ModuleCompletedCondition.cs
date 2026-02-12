namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires a specific module to be completed
/// </summary>
public sealed class ModuleCompletedCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.ModuleCompleted;
    public string RequiredModuleId { get; private set; }
    public decimal? CompletionPercentage { get; private set; }

    private ModuleCompletedCondition() : base()
    {
        RequiredModuleId = string.Empty;
    }

    private ModuleCompletedCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        string requiredModuleId,
        decimal? completionPercentage,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        RequiredModuleId = requiredModuleId;
        CompletionPercentage = completionPercentage;
    }

    public static ModuleCompletedCondition Create(
        PrerequisiteId prerequisiteId,
        string requiredModuleId,
        decimal? completionPercentage = 100,
        int displayOrder = 0)
    {
        return new ModuleCompletedCondition(
            ConditionId.New(),
            prerequisiteId,
            requiredModuleId,
            completionPercentage,
            displayOrder);
    }
}
