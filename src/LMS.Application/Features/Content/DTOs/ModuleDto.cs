namespace LMS.Application.Features.Content.DTOs;

/// <summary>
/// Complete module details with all properties
/// </summary>
public sealed record ModuleDto
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public string? Thumbnail { get; init; }
    public decimal EstimatedHours { get; init; }
    public Guid? ProductId { get; init; }
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public int StagesCount { get; init; }
    public List<StageListDto> Stages { get; init; } = [];
}

/// <summary>
/// Simplified module info for listings
/// </summary>
public sealed record ModuleListDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public decimal EstimatedHours { get; init; }
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public int StagesCount { get; init; }
}

/// <summary>
/// Input model for creating a module
/// </summary>
public sealed record CreateModuleDto
{
    public Guid CourseId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Thumbnail { get; init; }
    public decimal EstimatedHours { get; init; }
}

/// <summary>
/// Input model for updating a module
/// </summary>
public sealed record UpdateModuleDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Thumbnail { get; init; }
    public decimal EstimatedHours { get; init; }
    public bool IsActive { get; init; }
}
