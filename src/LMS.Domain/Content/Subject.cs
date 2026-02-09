using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Represents an academic subject (e.g., Math, Physics, Chemistry)
/// Reference data - typically seeded in database
/// </summary>
public sealed class Subject : Entity<Guid>
{
    /// <summary>
    /// Subject name in Arabic
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Subject name in English
    /// </summary>
    public string? NameEn { get; private set; }

    /// <summary>
    /// Icon URL or identifier
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Color code for UI display (hex format #RRGGBB)
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Indicates if this is a core subject (Math, Arabic, etc.)
    /// </summary>
    public bool IsCore { get; private set; }

    /// <summary>
    /// Indicates if this subject is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public short DisplayOrder { get; private set; }

    // EF Core constructor
    private Subject() : base()
    {
        Name = string.Empty;
    }

    private Subject(
        Guid id,
        string name,
        string? nameEn,
        short displayOrder,
        bool isCore = false)
        : base(id)
    {
        Name = name;
        NameEn = nameEn;
        DisplayOrder = displayOrder;
        IsCore = isCore;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new subject
    /// </summary>
    public static Result<Subject> Create(
        string name,
        string? nameEn,
        short displayOrder,
        bool isCore = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Subject>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (displayOrder < 0)
        {
            return Result<Subject>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var subject = new Subject(
            Guid.NewGuid(),
            name,
            nameEn,
            displayOrder,
            isCore
        );

        return Result<Subject>.Success(subject);
    }

    /// <summary>
    /// Updates subject details
    /// </summary>
    public Result Update(
        string name,
        string? nameEn,
        short displayOrder,
        bool isCore)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        Name = name;
        NameEn = nameEn;
        DisplayOrder = displayOrder;
        IsCore = isCore;

        return Result.Success();
    }

    /// <summary>
    /// Sets the subject icon
    /// </summary>
    public void SetIcon(string? icon) => Icon = icon;

    /// <summary>
    /// Sets the subject color (hex format #RRGGBB)
    /// </summary>
    public Result SetColor(string? color)
    {
        if (color != null && !System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$"))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Color = color;
        return Result.Success();
    }

    /// <summary>
    /// Deactivates the subject
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the subject
    /// </summary>
    public void Activate() => IsActive = true;
}
