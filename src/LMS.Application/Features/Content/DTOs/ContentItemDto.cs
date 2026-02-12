namespace LMS.Application.Features.Content.DTOs;

/// <summary>
/// Complete content item details with all properties
/// </summary>
public sealed record ContentItemDto
{
    public Guid Id { get; init; }
    public Guid StageId { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsFreePreview { get; init; }
    public int XpReward { get; init; }
    public int PurchasingPointsReward { get; init; }
    public Guid? ProductId { get; init; }
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }

    // Video-specific properties (null if not video)
    public string? ExternalVideoId { get; init; }
    public int? DurationSeconds { get; init; }
    public string? VideoStatus { get; init; }

    // File-specific properties (null if not file)
    public string? FileUrl { get; init; }
    public long? FileSizeBytes { get; init; }
    public string? FileType { get; init; }
}

/// <summary>
/// Simplified content item info for listings
/// </summary>
public sealed record ContentItemListDto
{
    public Guid Id { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsFreePreview { get; init; }
    public int XpReward { get; init; }
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public int? DurationSeconds { get; init; }
}

/// <summary>
/// Input model for creating video content
/// </summary>
public sealed record CreateVideoContentDto
{
    public Guid StageId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string ExternalVideoId { get; init; } = string.Empty;
    public int DurationSeconds { get; init; }
    public bool IsFreePreview { get; init; }
    public int XpReward { get; init; }
    public int PurchasingPointsReward { get; init; }
}

/// <summary>
/// Input model for creating file content
/// </summary>
public sealed record CreateFileContentDto
{
    public Guid StageId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string FileType { get; init; } = string.Empty;
    public bool IsFreePreview { get; init; }
    public int XpReward { get; init; }
    public int PurchasingPointsReward { get; init; }
}

/// <summary>
/// Input model for updating content item
/// </summary>
public sealed record UpdateContentItemDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsFreePreview { get; init; }
    public int XpReward { get; init; }
    public int PurchasingPointsReward { get; init; }
    public bool IsActive { get; init; }
}
