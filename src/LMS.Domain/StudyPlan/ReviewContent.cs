using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Represents a piece of content assigned for review in a review task
/// </summary>
public sealed class ReviewContent : Entity<Guid>
{
    /// <summary>
    /// Reference to the plan task
    /// </summary>
    public Guid PlanTaskId { get; private set; }

    /// <summary>
    /// Reference to the content item to review
    /// </summary>
    public Guid ContentItemId { get; private set; }

    /// <summary>
    /// Display order within the review task
    /// </summary>
    public short DisplayOrder { get; private set; }

    /// <summary>
    /// Whether this review item has been completed
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// When this review item was completed
    /// </summary>
    public DateTime? CompletedAtUtc { get; private set; }

    // EF Core constructor
    private ReviewContent() : base()
    {
    }

    private ReviewContent(
        Guid id,
        Guid planTaskId,
        Guid contentItemId,
        short displayOrder)
        : base(id)
    {
        PlanTaskId = planTaskId;
        ContentItemId = contentItemId;
        DisplayOrder = displayOrder;
        IsCompleted = false;
    }

    /// <summary>
    /// Factory method to create review content
    /// </summary>
    public static Result<ReviewContent> Create(
        Guid planTaskId,
        Guid contentItemId,
        short displayOrder)
    {
        if (planTaskId == Guid.Empty)
        {
            return Result<ReviewContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (contentItemId == Guid.Empty)
        {
            return Result<ReviewContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var reviewContent = new ReviewContent(
            Guid.NewGuid(),
            planTaskId,
            contentItemId,
            displayOrder
        );

        return Result<ReviewContent>.Success(reviewContent);
    }

    /// <summary>
    /// Marks this review item as completed
    /// </summary>
    public Result MarkAsCompleted()
    {
        if (IsCompleted)
        {
            return Result.Failure(
                Error.Conflict("REVIEW.ALREADY_COMPLETED")
            );
        }

        IsCompleted = true;
        CompletedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Resets completion status
    /// </summary>
    public void Reset()
    {
        IsCompleted = false;
        CompletedAtUtc = null;
    }
}
