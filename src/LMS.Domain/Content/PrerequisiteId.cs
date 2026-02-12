namespace LMS.Domain.Content;

public sealed record PrerequisiteId(Guid Value)
{
    public static PrerequisiteId New() => new(Guid.NewGuid());
    public static PrerequisiteId From(Guid value) => new(value);
    public static implicit operator Guid(PrerequisiteId id) => id.Value;
    public override string ToString() => Value.ToString();
}
