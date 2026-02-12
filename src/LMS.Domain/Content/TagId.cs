namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for Tag entity
/// </summary>
public sealed record TagId(Guid Value)
{
    /// <summary>
    /// Creates a new unique TagId
    /// </summary>
    public static TagId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a TagId from an existing Guid
    /// </summary>
    public static TagId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from TagId to Guid
    /// </summary>
    public static implicit operator Guid(TagId id) => id.Value;

    public override string ToString() => Value.ToString();
}
