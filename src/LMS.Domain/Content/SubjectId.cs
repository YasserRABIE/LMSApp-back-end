namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for Subject entity
/// </summary>
public sealed record SubjectId(Guid Value)
{
    /// <summary>
    /// Creates a new unique SubjectId
    /// </summary>
    public static SubjectId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a SubjectId from an existing Guid
    /// </summary>
    public static SubjectId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from SubjectId to Guid
    /// </summary>
    public static implicit operator Guid(SubjectId id) => id.Value;

    public override string ToString() => Value.ToString();
}
