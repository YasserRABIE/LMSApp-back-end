namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for Course entity
/// </summary>
public sealed record CourseId(Guid Value)
{
    /// <summary>
    /// Creates a new unique CourseId
    /// </summary>
    public static CourseId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a CourseId from an existing Guid
    /// </summary>
    public static CourseId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from CourseId to Guid
    /// </summary>
    public static implicit operator Guid(CourseId id) => id.Value;

    public override string ToString() => Value.ToString();
}
