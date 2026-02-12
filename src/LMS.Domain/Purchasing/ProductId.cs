namespace LMS.Domain.Purchasing;

/// <summary>
/// Strongly-typed identifier for Product entity
/// </summary>
public sealed record ProductId(Guid Value)
{
    /// <summary>
    /// Creates a new unique ProductId
    /// </summary>
    public static ProductId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a ProductId from an existing Guid
    /// </summary>
    public static ProductId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from ProductId to Guid
    /// </summary>
    public static implicit operator Guid(ProductId id) => id.Value;

    public override string ToString() => Value.ToString();
}
