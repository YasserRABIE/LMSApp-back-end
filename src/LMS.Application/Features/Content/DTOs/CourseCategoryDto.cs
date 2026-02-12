namespace LMS.Application.Features.Content.DTOs;

public sealed record CourseCategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}
