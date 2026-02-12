namespace LMS.Application.Features.Content.DTOs;

public sealed record SubjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public bool IsCore { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

public sealed record SubjectListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public bool IsCore { get; init; }
    public bool IsActive { get; init; }
}
