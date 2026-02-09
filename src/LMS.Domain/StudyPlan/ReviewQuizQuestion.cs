using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Represents a question in a review quiz
/// </summary>
public sealed class ReviewQuizQuestion : Entity<Guid>
{
    /// <summary>
    /// Reference to the review quiz
    /// </summary>
    public Guid ReviewQuizId { get; private set; }

    /// <summary>
    /// Reference to the question
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Display order in the quiz
    /// </summary>
    public short DisplayOrder { get; private set; }

    // EF Core constructor
    private ReviewQuizQuestion() : base()
    {
    }

    private ReviewQuizQuestion(
        Guid id,
        Guid reviewQuizId,
        Guid questionId,
        short displayOrder)
        : base(id)
    {
        ReviewQuizId = reviewQuizId;
        QuestionId = questionId;
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Factory method to create a review quiz question
    /// </summary>
    public static Result<ReviewQuizQuestion> Create(
        Guid reviewQuizId,
        Guid questionId,
        short displayOrder)
    {
        if (reviewQuizId == Guid.Empty)
        {
            return Result<ReviewQuizQuestion>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (questionId == Guid.Empty)
        {
            return Result<ReviewQuizQuestion>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var reviewQuizQuestion = new ReviewQuizQuestion(
            Guid.NewGuid(),
            reviewQuizId,
            questionId,
            displayOrder
        );

        return Result<ReviewQuizQuestion>.Success(reviewQuizQuestion);
    }
}
