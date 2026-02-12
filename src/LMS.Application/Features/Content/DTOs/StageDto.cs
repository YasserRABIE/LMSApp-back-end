namespace LMS.Application.Features.Content.DTOs;

/// <summary>
/// Complete stage details with all properties
/// </summary>
public sealed record StageDto
{
    public Guid Id { get; init; }
    public Guid ModuleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public string Visibility { get; init; } = string.Empty;
    public Guid? ProductId { get; init; }
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public int ContentItemsCount { get; init; }
    public List<ContentItemListDto> ContentItems { get; init; } = [];
}

/// <summary>
/// Simplified stage info for listings
/// </summary>
public sealed record StageListDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public string Visibility { get; init; } = string.Empty;
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public int ContentItemsCount { get; init; }
}

/// <summary>
/// Input model for creating a stage
/// </summary>
public sealed record CreateStageDto
{
    public Guid ModuleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
}

/// <summary>
/// Input model for updating a stage
/// </summary>
public sealed record UpdateStageDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Visibility { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
