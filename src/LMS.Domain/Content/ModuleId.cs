namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for Module entity
/// </summary>
public sealed record ModuleId(Guid Value)
{
    /// <summary>
    /// Creates a new unique ModuleId
    /// </summary>
    public static ModuleId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a ModuleId from an existing Guid
    /// </summary>
    public static ModuleId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from ModuleId to Guid
    /// </summary>
    public static implicit operator Guid(ModuleId id) => id.Value;

    public override string ToString() => Value.ToString();
}
