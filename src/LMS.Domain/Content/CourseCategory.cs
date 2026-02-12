using LMS.Domain.Common;

namespace LMS.Domain.Content;

public sealed class CourseCategory : Entity<CourseCategoryId>
{
    public string Name { get; private set; }
    public int DisplayOrder { get; private set; }

    private CourseCategory() : base()
    {
        Name = string.Empty;
    }

    private CourseCategory(CourseCategoryId id, string name, int displayOrder) : base(id)
    {
        Name = name;
        DisplayOrder = displayOrder;
    }

    public static Result<CourseCategory> Create(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<CourseCategory>.Failure(Error.Validation(ErrorCodes.CourseCategory.NameRequired));

        if (name.Length > 50)
            return Result<CourseCategory>.Failure(Error.Validation(ErrorCodes.CourseCategory.NameTooLong));

        var courseCategory = new CourseCategory(CourseCategoryId.New(), name.Trim(), displayOrder);
        return Result<CourseCategory>.Success(courseCategory);
    }
}
