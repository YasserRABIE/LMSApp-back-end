namespace LMS.Application.Features.Content.DTOs;

/// <summary>
/// Complete course details with all properties
/// </summary>
public sealed record CourseDto
{
    public Guid Id { get; init; }
    public Guid TeacherId { get; init; }
    public string TeacherName { get; init; } = string.Empty;
    public Guid SubjectId { get; init; }
    public string SubjectName { get; init; } = string.Empty;
    public Guid StudyLevelId { get; init; }
    public Guid TrackId { get; init; }
    public Guid SchoolTypeId { get; init; }
    public Guid CourseCategoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public Guid? ProductId { get; init; }
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public int ModulesCount { get; init; }
    public List<ModuleListDto> Modules { get; init; } = [];
}

/// <summary>
/// Simplified course info for listings
/// </summary>
public sealed record CourseListDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public string TeacherName { get; init; } = string.Empty;
    public string SubjectName { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public int ModulesCount { get; init; }
}

/// <summary>
/// Input model for creating a course
/// </summary>
public sealed record CreateCourseDto
{
    public Guid SubjectId { get; init; }
    public Guid StudyLevelId { get; init; }
    public Guid TrackId { get; init; }
    public Guid SchoolTypeId { get; init; }
    public Guid CourseCategoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
}

/// <summary>
/// Input model for updating a course
/// </summary>
public sealed record UpdateCourseDto
{
    public Guid SubjectId { get; init; }
    public Guid SchoolTypeId { get; init; }
    public Guid CourseCategoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
}
