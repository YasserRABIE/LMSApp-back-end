namespace LMS.Domain.Content;

public sealed record ConditionId(Guid Value)
{
    public static ConditionId New() => new(Guid.NewGuid());
    public static ConditionId From(Guid value) => new(value);
    public static implicit operator Guid(ConditionId id) => id.Value;
    public override string ToString() => Value.ToString();
}
