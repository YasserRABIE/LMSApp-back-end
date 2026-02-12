namespace LMS.Application.Features.Content.DTOs;

/// <summary>
/// Complete prerequisite details with conditions and actions
/// </summary>
public sealed record PrerequisiteDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TargetEntityType { get; init; } = string.Empty;
    public string TargetEntityId { get; init; } = string.Empty;
    public string LogicOperator { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<ConditionDto> Conditions { get; init; } = [];
    public List<ActionDto> Actions { get; init; } = [];
}

/// <summary>
/// Prerequisite condition DTO
/// </summary>
public sealed record ConditionDto
{
    public Guid Id { get; init; }
    public string ConditionType { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public Dictionary<string, object> Parameters { get; init; } = new();
}

/// <summary>
/// Prerequisite action DTO
/// </summary>
public sealed record ActionDto
{
    public Guid Id { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public Dictionary<string, object> Parameters { get; init; } = new();
}

/// <summary>
/// Input model for creating a prerequisite
/// </summary>
public sealed record CreatePrerequisiteDto
{
    public string Name { get; init; } = string.Empty;
    public string TargetEntityType { get; init; } = string.Empty;
    public string TargetEntityId { get; init; } = string.Empty;
    public string LogicOperator { get; init; } = "And";
}

/// <summary>
/// Input model for updating a prerequisite
/// </summary>
public sealed record UpdatePrerequisiteDto
{
    public string Name { get; init; } = string.Empty;
    public string LogicOperator { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

/// <summary>
/// Input for adding condition to prerequisite
/// </summary>
public sealed record AddConditionDto
{
    public string ConditionType { get; init; } = string.Empty;
    public Dictionary<string, object> Parameters { get; init; } = new();
}

/// <summary>
/// Input for adding action to prerequisite
/// </summary>
public sealed record AddActionDto
{
    public string ActionType { get; init; } = string.Empty;
    public Dictionary<string, object> Parameters { get; init; } = new();
}

/// <summary>
/// Result of prerequisite evaluation
/// </summary>
public sealed record PrerequisiteEvaluationResult
{
    public Guid PrerequisiteId { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsMet { get; init; }
    public List<string> UnmetConditions { get; init; } = [];
    public List<ActionDto> ActionsToExecute { get; init; } = [];
}
