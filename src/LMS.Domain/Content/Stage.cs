using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Represents a chapter/stage within a module
/// Stages organize content items into logical sections
/// </summary>
public sealed class Stage : Entity<Guid>
{
    /// <summary>
    /// Reference to the parent module
    /// </summary>
    public Guid ModuleId { get; private set; }

    /// <summary>
    /// Stage title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Stage description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Display order within the module
    /// </summary>
    public short DisplayOrder { get; private set; }

    /// <summary>
    /// Stage visibility status
    /// </summary>
    public Visibility Visibility { get; private set; }

    /// <summary>
    /// Indicates if this stage is active
    /// </summary>
    public bool IsActive { get; private set; }

    // EF Core constructor
    private Stage() : base()
    {
        Title = string.Empty;
    }

    private Stage(
        Guid id,
        Guid moduleId,
        string title,
        string? description,
        short displayOrder)
        : base(id)
    {
        ModuleId = moduleId;
        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        Visibility = Visibility.Draft;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new stage
    /// </summary>
    public static Result<Stage> Create(
        Guid moduleId,
        string title,
        string? description,
        short displayOrder)
    {
        if (moduleId == Guid.Empty)
        {
            return Result<Stage>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Stage>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result<Stage>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var stage = new Stage(
            Guid.NewGuid(),
            moduleId,
            title,
            description,
            displayOrder
        );

        return Result<Stage>.Success(stage);
    }

    /// <summary>
    /// Updates stage details
    /// </summary>
    public Result UpdateDetails(
        string title,
        string? description,
        short displayOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Title = title;
        Description = description;
        DisplayOrder = displayOrder;

        return Result.Success();
    }

    /// <summary>
    /// Publishes the stage (makes it visible to students)
    /// </summary>
    public Result Publish()
    {
        if (Visibility == Visibility.Published)
        {
            return Result.Failure(
                Error.Conflict("STAGE.ALREADY_PUBLISHED")
            );
        }

        Visibility = Visibility.Published;
        return Result.Success();
    }

    /// <summary>
    /// Unpublishes the stage
    /// </summary>
    public void Unpublish() => Visibility = Visibility.Draft;

    /// <summary>
    /// Hides the stage completely
    /// </summary>
    public void Hide() => Visibility = Visibility.Hidden;

    /// <summary>
    /// Deactivates the stage
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the stage
    /// </summary>
    public void Activate() => IsActive = true;
}
