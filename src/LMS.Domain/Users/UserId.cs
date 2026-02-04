namespace LMS.Domain.Users;

/// <summary>
/// Strongly-typed identifier for User entity
/// </summary>
public sealed record UserId(Guid Value)
{
    /// <summary>
    /// Creates a new unique UserId
    /// </summary>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a UserId from an existing Guid
    /// </summary>
    public static UserId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from UserId to Guid
    /// </summary>
    public static implicit operator Guid(UserId userId) => userId.Value;

    public override string ToString() => Value.ToString();
}
