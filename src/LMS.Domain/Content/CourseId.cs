namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for Course entity
/// </summary>
public readonly record struct CourseId(Guid Value)
{
    public static CourseId New() => new(Guid.NewGuid());
    public static CourseId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
