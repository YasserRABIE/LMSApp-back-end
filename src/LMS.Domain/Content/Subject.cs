using LMS.Domain.Common;

namespace LMS.Domain.Content;

public sealed class Subject : Entity<SubjectId>
{
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public string Color { get; private set; }
    public bool IsCore { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private Subject() : base()
    {
        Name = string.Empty;
        Icon = string.Empty;
        Color = string.Empty;
    }

    private Subject(SubjectId id, string name, string icon, string color, bool isCore, int displayOrder) : base(id)
    {
        Name = name;
        Icon = icon;
        Color = color;
        IsCore = isCore;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public static Result<Subject> Create(string name, string icon, string color, bool isCore, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Subject>.Failure(Error.Validation(ErrorCodes.Subject.NameRequired));

        if (name.Length > 100)
            return Result<Subject>.Failure(Error.Validation(ErrorCodes.Subject.NameTooLong));

        if (string.IsNullOrWhiteSpace(icon))
            return Result<Subject>.Failure(Error.Validation(ErrorCodes.Subject.IconRequired));

        if (string.IsNullOrWhiteSpace(color))
            return Result<Subject>.Failure(Error.Validation(ErrorCodes.Subject.ColorRequired));

        if (displayOrder < 0)
            return Result<Subject>.Failure(Error.Validation(ErrorCodes.Subject.InvalidDisplayOrder));

        var subject = new Subject(SubjectId.New(), name.Trim(), icon.Trim(), color.Trim(), isCore, displayOrder);
        return Result<Subject>.Success(subject);
    }

    public Result UpdateDetails(string name, string icon, string color, bool isCore, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation(ErrorCodes.Subject.NameRequired));

        if (name.Length > 100)
            return Result.Failure(Error.Validation(ErrorCodes.Subject.NameTooLong));

        if (string.IsNullOrWhiteSpace(icon))
            return Result.Failure(Error.Validation(ErrorCodes.Subject.IconRequired));

        if (string.IsNullOrWhiteSpace(color))
            return Result.Failure(Error.Validation(ErrorCodes.Subject.ColorRequired));

        if (displayOrder < 0)
            return Result.Failure(Error.Validation(ErrorCodes.Subject.InvalidDisplayOrder));

        Name = name.Trim();
        Icon = icon.Trim();
        Color = color.Trim();
        IsCore = isCore;
        DisplayOrder = displayOrder;

        return Result.Success();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
