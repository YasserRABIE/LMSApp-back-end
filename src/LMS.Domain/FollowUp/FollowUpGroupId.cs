namespace LMS.Domain.FollowUp;

/// <summary>
/// Strongly-typed identifier for FollowUpGroup entity
/// </summary>
public readonly record struct FollowUpGroupId(Guid Value)
{
    public static FollowUpGroupId New() => new(Guid.NewGuid());
    public static FollowUpGroupId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
