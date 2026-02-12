using LMS.Domain.Common;

namespace LMS.Domain.Content;

public sealed class Tag : Entity<TagId>
{
    public string Name { get; private set; }

    private Tag() : base() { Name = string.Empty; }

    private Tag(TagId id, string name) : base(id)
    {
        Name = name;
    }

    public static Result<Tag> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Tag>.Failure(Error.Validation(ErrorCodes.Tag.NameRequired));

        if (name.Length > 50)
            return Result<Tag>.Failure(Error.Validation(ErrorCodes.Tag.NameTooLong));

        var tag = new Tag(TagId.New(), name.Trim());
        return Result<Tag>.Success(tag);
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation(ErrorCodes.Tag.NameRequired));

        if (name.Length > 50)
            return Result.Failure(Error.Validation(ErrorCodes.Tag.NameTooLong));

        Name = name.Trim();
        return Result.Success();
    }
}
