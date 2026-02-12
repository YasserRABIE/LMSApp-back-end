namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for ContentItem entity
/// </summary>
public sealed record ContentItemId(Guid Value)
{
    /// <summary>
    /// Creates a new unique ContentItemId
    /// </summary>
    public static ContentItemId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a ContentItemId from an existing Guid
    /// </summary>
    public static ContentItemId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from ContentItemId to Guid
    /// </summary>
    public static implicit operator Guid(ContentItemId id) => id.Value;

    public override string ToString() => Value.ToString();
}
