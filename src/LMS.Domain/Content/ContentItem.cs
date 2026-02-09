using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Represents a piece of content within a stage
/// Can be a video, file, quiz, assignment, or workshop
/// </summary>
public sealed class ContentItem : Entity<Guid>
{
    /// <summary>
    /// Reference to the parent stage (can be null for standalone content)
    /// </summary>
    public Guid? StageId { get; private set; }

    /// <summary>
    /// Reference to the course (always required)
    /// </summary>
    public CourseId CourseId { get; private set; }

    /// <summary>
    /// Type of content
    /// </summary>
    public ContentType ContentType { get; private set; }

    /// <summary>
    /// Content title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Content description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Thumbnail image URL
    /// </summary>
    public string? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Display order within the stage
    /// </summary>
    public short DisplayOrder { get; private set; }

    /// <summary>
    /// Indicates if this content is available as a free preview
    /// </summary>
    public bool IsFreePreview { get; private set; }

    /// <summary>
    /// Indicates if this content can be purchased separately
    /// </summary>
    public bool IsStandalone { get; private set; }

    /// <summary>
    /// Price if purchased as standalone (required when IsStandalone is true)
    /// </summary>
    public decimal? StandalonePrice { get; private set; }

    /// <summary>
    /// XP points awarded upon completion
    /// </summary>
    public short XpReward { get; private set; }

    /// <summary>
    /// Game points awarded upon completion
    /// </summary>
    public short PointsReward { get; private set; }

    /// <summary>
    /// Estimated time to complete in minutes
    /// </summary>
    public short? EstimatedMinutes { get; private set; }

    /// <summary>
    /// Content visibility status
    /// </summary>
    public Visibility Visibility { get; private set; }

    /// <summary>
    /// Indicates if this content is active
    /// </summary>
    public bool IsActive { get; private set; }

    // EF Core constructor
    private ContentItem() : base()
    {
        Title = string.Empty;
        CourseId = CourseId.New();
    }

    private ContentItem(
        Guid id,
        Guid? stageId,
        CourseId courseId,
        ContentType contentType,
        string title,
        string? description,
        short displayOrder,
        bool isFreePreview = false,
        bool isStandalone = false,
        decimal? standalonePrice = null,
        short xpReward = 0,
        short pointsReward = 0,
        short? estimatedMinutes = null)
        : base(id)
    {
        StageId = stageId;
        CourseId = courseId;
        ContentType = contentType;
        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        IsFreePreview = isFreePreview;
        IsStandalone = isStandalone;
        StandalonePrice = standalonePrice;
        XpReward = xpReward;
        PointsReward = pointsReward;
        EstimatedMinutes = estimatedMinutes;
        Visibility = Visibility.Draft;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new content item
    /// </summary>
    public static Result<ContentItem> Create(
        Guid? stageId,
        CourseId courseId,
        ContentType contentType,
        string title,
        string? description,
        short displayOrder,
        bool isFreePreview = false,
        bool isStandalone = false,
        decimal? standalonePrice = null,
        short xpReward = 0,
        short pointsReward = 0,
        short? estimatedMinutes = null)
    {
        if (courseId.Value == Guid.Empty)
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (isStandalone && standalonePrice == null)
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (standalonePrice.HasValue && standalonePrice.Value < 0)
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (xpReward < 0)
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (pointsReward < 0)
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (estimatedMinutes.HasValue && estimatedMinutes.Value <= 0)
        {
            return Result<ContentItem>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var contentItem = new ContentItem(
            Guid.NewGuid(),
            stageId,
            courseId,
            contentType,
            title,
            description,
            displayOrder,
            isFreePreview,
            isStandalone,
            standalonePrice,
            xpReward,
            pointsReward,
            estimatedMinutes
        );

        return Result<ContentItem>.Success(contentItem);
    }

    /// <summary>
    /// Updates content item details
    /// </summary>
    public Result UpdateDetails(
        string title,
        string? description,
        short displayOrder,
        short? estimatedMinutes)
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

        if (estimatedMinutes.HasValue && estimatedMinutes.Value <= 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        EstimatedMinutes = estimatedMinutes;

        return Result.Success();
    }

    /// <summary>
    /// Sets the content thumbnail
    /// </summary>
    public void SetThumbnail(string? thumbnailUrl) => ThumbnailUrl = thumbnailUrl;

    /// <summary>
    /// Enables standalone purchasing with a price
    /// </summary>
    public Result EnableStandalone(decimal price)
    {
        if (price < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        IsStandalone = true;
        StandalonePrice = price;

        return Result.Success();
    }

    /// <summary>
    /// Disables standalone purchasing
    /// </summary>
    public void DisableStandalone()
    {
        IsStandalone = false;
        StandalonePrice = null;
    }

    /// <summary>
    /// Marks this content as a free preview
    /// </summary>
    public void EnableFreePreview() => IsFreePreview = true;

    /// <summary>
    /// Removes free preview status
    /// </summary>
    public void DisableFreePreview() => IsFreePreview = false;

    /// <summary>
    /// Sets the gamification rewards
    /// </summary>
    public Result SetRewards(short xpReward, short pointsReward)
    {
        if (xpReward < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (pointsReward < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        XpReward = xpReward;
        PointsReward = pointsReward;

        return Result.Success();
    }

    /// <summary>
    /// Moves this content to a different stage
    /// </summary>
    public void MoveToStage(Guid? stageId) => StageId = stageId;

    /// <summary>
    /// Publishes the content (makes it visible to students)
    /// </summary>
    public Result Publish()
    {
        if (Visibility == Visibility.Published)
        {
            return Result.Failure(
                Error.Conflict("CONTENT.ALREADY_PUBLISHED")
            );
        }

        Visibility = Visibility.Published;
        return Result.Success();
    }

    /// <summary>
    /// Unpublishes the content
    /// </summary>
    public void Unpublish() => Visibility = Visibility.Draft;

    /// <summary>
    /// Hides the content completely
    /// </summary>
    public void Hide() => Visibility = Visibility.Hidden;

    /// <summary>
    /// Deactivates the content
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the content
    /// </summary>
    public void Activate() => IsActive = true;
}
