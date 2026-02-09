using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Join entity linking assessments to questions with specific marks
/// </summary>
public sealed class AssessmentQuestion : Entity<Guid>
{
    /// <summary>
    /// Reference to the assessment
    /// </summary>
    public AssessmentId AssessmentId { get; private set; }

    /// <summary>
    /// Reference to the question
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Marks allocated for this question in this assessment
    /// </summary>
    public decimal Marks { get; private set; }

    /// <summary>
    /// Display order for this question in the assessment
    /// </summary>
    public short DisplayOrder { get; private set; }

    // EF Core constructor
    private AssessmentQuestion() : base()
    {
        AssessmentId = AssessmentId.New();
    }

    private AssessmentQuestion(
        Guid id,
        AssessmentId assessmentId,
        Guid questionId,
        decimal marks,
        short displayOrder)
        : base(id)
    {
        AssessmentId = assessmentId;
        QuestionId = questionId;
        Marks = marks;
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Factory method to create a new assessment-question link
    /// </summary>
    public static Result<AssessmentQuestion> Create(
        AssessmentId assessmentId,
        Guid questionId,
        decimal marks,
        short displayOrder)
    {
        if (assessmentId.Value == Guid.Empty)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (questionId == Guid.Empty)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (marks <= 0)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var assessmentQuestion = new AssessmentQuestion(
            Guid.NewGuid(),
            assessmentId,
            questionId,
            marks,
            displayOrder
        );

        return Result<AssessmentQuestion>.Success(assessmentQuestion);
    }

    /// <summary>
    /// Updates the marks for this question
    /// </summary>
    public Result UpdateMarks(decimal marks)
    {
        if (marks <= 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Marks = marks;
        return Result.Success();
    }

    /// <summary>
    /// Updates the display order
    /// </summary>
    public void UpdateDisplayOrder(short displayOrder) => DisplayOrder = displayOrder;
}
