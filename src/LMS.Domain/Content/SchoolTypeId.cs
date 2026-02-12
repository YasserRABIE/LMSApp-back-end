namespace LMS.Domain.Content;

public sealed record SchoolTypeId(Guid Value)
{
    public static SchoolTypeId New() => new(Guid.NewGuid());
    public static SchoolTypeId From(Guid value) => new(value);
    public static implicit operator Guid(SchoolTypeId id) => id.Value;
    public override string ToString() => Value.ToString();
}
