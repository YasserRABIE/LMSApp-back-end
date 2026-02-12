namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for Stage entity
/// </summary>
public sealed record StageId(Guid Value)
{
    /// <summary>
    /// Creates a new unique StageId
    /// </summary>
    public static StageId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a StageId from an existing Guid
    /// </summary>
    public static StageId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from StageId to Guid
    /// </summary>
    public static implicit operator Guid(StageId id) => id.Value;

    public override string ToString() => Value.ToString();
}
