using LMS.Domain.Common;

namespace LMS.Domain.Content;

public sealed class SchoolType : Entity<SchoolTypeId>
{
    public string Name { get; private set; }
    public int DisplayOrder { get; private set; }

    private SchoolType() : base()
    {
        Name = string.Empty;
    }

    private SchoolType(SchoolTypeId id, string name, int displayOrder) : base(id)
    {
        Name = name;
        DisplayOrder = displayOrder;
    }

    public static Result<SchoolType> Create(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<SchoolType>.Failure(Error.Validation(ErrorCodes.SchoolType.NameRequired));

        if (name.Length > 50)
            return Result<SchoolType>.Failure(Error.Validation(ErrorCodes.SchoolType.NameTooLong));

        var schoolType = new SchoolType(SchoolTypeId.New(), name.Trim(), displayOrder);
        return Result<SchoolType>.Success(schoolType);
    }
}
