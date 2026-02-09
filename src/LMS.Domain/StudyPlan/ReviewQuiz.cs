using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Represents a review quiz for a specific stage
/// </summary>
public sealed class ReviewQuiz : Entity<Guid>
{
    /// <summary>
    /// Reference to the plan task
    /// </summary>
    public Guid PlanTaskId { get; private set; }

    /// <summary>
    /// Reference to the stage being reviewed
    /// </summary>
    public Guid StageId { get; private set; }

    // EF Core constructor
    private ReviewQuiz() : base()
    {
    }

    private ReviewQuiz(
        Guid id,
        Guid planTaskId,
        Guid stageId)
        : base(id)
    {
        PlanTaskId = planTaskId;
        StageId = stageId;
    }

    /// <summary>
    /// Factory method to create a review quiz
    /// </summary>
    public static Result<ReviewQuiz> Create(
        Guid planTaskId,
        Guid stageId)
    {
        if (planTaskId == Guid.Empty)
        {
            return Result<ReviewQuiz>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (stageId == Guid.Empty)
        {
            return Result<ReviewQuiz>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var reviewQuiz = new ReviewQuiz(
            Guid.NewGuid(),
            planTaskId,
            stageId
        );

        return Result<ReviewQuiz>.Success(reviewQuiz);
    }
}
