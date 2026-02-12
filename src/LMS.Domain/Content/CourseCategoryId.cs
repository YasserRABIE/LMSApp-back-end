namespace LMS.Domain.Content;

public sealed record CourseCategoryId(Guid Value)
{
    public static CourseCategoryId New() => new(Guid.NewGuid());
    public static CourseCategoryId From(Guid value) => new(value);
    public static implicit operator Guid(CourseCategoryId id) => id.Value;
    public override string ToString() => Value.ToString();
}
