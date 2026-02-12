namespace LMS.Application.Features.Content.DTOs;

public sealed record TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
