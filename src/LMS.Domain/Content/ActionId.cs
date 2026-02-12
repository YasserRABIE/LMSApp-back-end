namespace LMS.Domain.Content;

public sealed record ActionId(Guid Value)
{
    public static ActionId New() => new(Guid.NewGuid());
    public static ActionId From(Guid value) => new(value);
    public static implicit operator Guid(ActionId id) => id.Value;
    public override string ToString() => Value.ToString();
}
