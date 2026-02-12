using LMS.Domain.Common;
using LMS.Domain.Content.Actions;
using LMS.Domain.Content.Conditions;

namespace LMS.Domain.Content;

/// <summary>
/// Represents a prerequisite rule that controls access to content
/// This is a separate aggregate root from Module/Stage/ContentItem
/// </summary>
public sealed class Prerequisite : Entity<PrerequisiteId>
{
    private readonly List<PrerequisiteCondition> _conditions = [];
    private readonly List<PrerequisiteAction> _actions = [];

    public string Name { get; private set; }
    public string TargetEntityId { get; private set; }
    public TargetEntityType TargetEntityType { get; private set; }
    public LogicOperator LogicOperator { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<PrerequisiteCondition> Conditions => _conditions.AsReadOnly();
    public IReadOnlyCollection<PrerequisiteAction> Actions => _actions.AsReadOnly();

    private Prerequisite() : base()
    {
        Name = string.Empty;
        TargetEntityId = string.Empty;
    }

    private Prerequisite(
        PrerequisiteId id,
        string name,
        string targetEntityId,
        TargetEntityType targetEntityType,
        LogicOperator logicOperator,
        int displayOrder) : base(id)
    {
        Name = name;
        TargetEntityId = targetEntityId;
        TargetEntityType = targetEntityType;
        LogicOperator = logicOperator;
        DisplayOrder = displayOrder;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Prerequisite> Create(
        string name,
        string targetEntityId,
        TargetEntityType targetEntityType,
        LogicOperator logicOperator = LogicOperator.And,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Prerequisite>.Failure(Error.Validation(ErrorCodes.Prerequisite.NameRequired));

        if (name.Length > 200)
            return Result<Prerequisite>.Failure(Error.Validation(ErrorCodes.Prerequisite.NameTooLong));

        if (string.IsNullOrWhiteSpace(targetEntityId))
            return Result<Prerequisite>.Failure(Error.Validation(ErrorCodes.Prerequisite.InvalidTargetType));

        var prerequisite = new Prerequisite(
            PrerequisiteId.New(),
            name.Trim(),
            targetEntityId,
            targetEntityType,
            logicOperator,
            displayOrder);

        return Result<Prerequisite>.Success(prerequisite);
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation(ErrorCodes.Prerequisite.NameRequired));

        if (name.Length > 200)
            return Result.Failure(Error.Validation(ErrorCodes.Prerequisite.NameTooLong));

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void UpdateLogicOperator(LogicOperator logicOperator)
    {
        LogicOperator = logicOperator;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result AddCondition(PrerequisiteCondition condition)
    {
        if (condition == null)
            return Result.Failure(Error.Validation(ErrorCodes.Prerequisite.InvalidConditionParameters));

        _conditions.Add(condition);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AddAction(PrerequisiteAction action)
    {
        if (action == null)
            return Result.Failure(Error.Validation(ErrorCodes.Prerequisite.InvalidActionParameters));

        _actions.Add(action);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveCondition(ConditionId conditionId)
    {
        var condition = _conditions.FirstOrDefault(c => c.Id == conditionId);
        if (condition == null)
            return Result.Failure(Error.NotFound(ErrorCodes.Prerequisite.NotFound));

        _conditions.Remove(condition);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveAction(ActionId actionId)
    {
        var action = _actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            return Result.Failure(Error.NotFound(ErrorCodes.Prerequisite.NotFound));

        _actions.Remove(action);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result Validate()
    {
        if (!_conditions.Any())
            return Result.Failure(Error.Validation(ErrorCodes.Prerequisite.NoConditions));

        if (!_actions.Any())
            return Result.Failure(Error.Validation(ErrorCodes.Prerequisite.NoActions));

        return Result.Success();
    }
}
