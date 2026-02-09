using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Represents a monthly unit within a course
/// Modules can be purchased individually or as part of the full course
/// </summary>
public sealed class Module : Entity<Guid>
{
    /// <summary>
    /// Reference to the parent course
    /// </summary>
    public CourseId CourseId { get; private set; }

    /// <summary>
    /// Module title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Module description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Thumbnail image URL
    /// </summary>
    public string? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Price for module without follow-up support
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Price for module with follow-up support (if available)
    /// </summary>
    public decimal? PriceWithFollowUp { get; private set; }

    /// <summary>
    /// Indicates if follow-up support option is available for this module
    /// </summary>
    public bool HasFollowUpOption { get; private set; }

    /// <summary>
    /// Display order within the course
    /// </summary>
    public short DisplayOrder { get; private set; }

    /// <summary>
    /// Module visibility status
    /// </summary>
    public Visibility Visibility { get; private set; }

    /// <summary>
    /// Indicates if this module is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Expected duration to complete this module (in days)
    /// </summary>
    public short ExpectedDurationDays { get; private set; }

    // EF Core constructor
    private Module() : base()
    {
        Title = string.Empty;
        CourseId = CourseId.New();
    }

    private Module(
        Guid id,
        CourseId courseId,
        string title,
        string? description,
        decimal price,
        short displayOrder,
        short expectedDurationDays = 30)
        : base(id)
    {
        CourseId = courseId;
        Title = title;
        Description = description;
        Price = price;
        DisplayOrder = displayOrder;
        ExpectedDurationDays = expectedDurationDays;
        Visibility = Visibility.Draft;
        IsActive = true;
        HasFollowUpOption = false;
    }

    /// <summary>
    /// Factory method to create a new module
    /// </summary>
    public static Result<Module> Create(
        CourseId courseId,
        string title,
        string? description,
        decimal price,
        short displayOrder,
        short expectedDurationDays = 30)
    {
        if (courseId.Value == Guid.Empty)
        {
            return Result<Module>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Module>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result<Module>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (price < 0)
        {
            return Result<Module>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (expectedDurationDays <= 0)
        {
            return Result<Module>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var module = new Module(
            Guid.NewGuid(),
            courseId,
            title,
            description,
            price,
            displayOrder,
            expectedDurationDays
        );

        return Result<Module>.Success(module);
    }

    /// <summary>
    /// Updates module details
    /// </summary>
    public Result UpdateDetails(
        string title,
        string? description,
        decimal price,
        short displayOrder,
        short expectedDurationDays)
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

        if (price < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (expectedDurationDays <= 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Title = title;
        Description = description;
        Price = price;
        DisplayOrder = displayOrder;
        ExpectedDurationDays = expectedDurationDays;

        return Result.Success();
    }

    /// <summary>
    /// Sets the module thumbnail
    /// </summary>
    public void SetThumbnail(string? thumbnailUrl) => ThumbnailUrl = thumbnailUrl;

    /// <summary>
    /// Enables follow-up support option with pricing
    /// </summary>
    public Result EnableFollowUp(decimal priceWithFollowUp)
    {
        if (priceWithFollowUp < Price)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        HasFollowUpOption = true;
        PriceWithFollowUp = priceWithFollowUp;

        return Result.Success();
    }

    /// <summary>
    /// Disables follow-up support option
    /// </summary>
    public void DisableFollowUp()
    {
        HasFollowUpOption = false;
        PriceWithFollowUp = null;
    }

    /// <summary>
    /// Publishes the module (makes it visible to students)
    /// </summary>
    public Result Publish()
    {
        if (Visibility == Visibility.Published)
        {
            return Result.Failure(
                Error.Conflict("MODULE.ALREADY_PUBLISHED")
            );
        }

        Visibility = Visibility.Published;
        return Result.Success();
    }

    /// <summary>
    /// Unpublishes the module
    /// </summary>
    public void Unpublish() => Visibility = Visibility.Draft;

    /// <summary>
    /// Hides the module completely
    /// </summary>
    public void Hide() => Visibility = Visibility.Hidden;

    /// <summary>
    /// Deactivates the module
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the module
    /// </summary>
    public void Activate() => IsActive = true;
}
