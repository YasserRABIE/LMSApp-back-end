using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a question in a question bank
/// Can be MCQ, True/False, or File Upload type
/// </summary>
public sealed class Question : Entity<Guid>
{
    /// <summary>
    /// Reference to the question bank
    /// </summary>
    public Guid QuestionBankId { get; private set; }

    /// <summary>
    /// Type of question
    /// </summary>
    public QuestionType QuestionType { get; private set; }

    /// <summary>
    /// Question text
    /// </summary>
    public string QuestionText { get; private set; }

    /// <summary>
    /// Optional image URL for the question
    /// </summary>
    public string? QuestionImageUrl { get; private set; }

    /// <summary>
    /// Question difficulty level
    /// </summary>
    public Difficulty Difficulty { get; private set; }

    /// <summary>
    /// Default marks/points for this question
    /// </summary>
    public decimal DefaultMarks { get; private set; }

    /// <summary>
    /// Explanation of the correct answer
    /// </summary>
    public string? Explanation { get; private set; }

    /// <summary>
    /// Indicates if this question is active
    /// </summary>
    public bool IsActive { get; private set; }

    // EF Core constructor
    private Question() : base()
    {
        QuestionText = string.Empty;
    }

    private Question(
        Guid id,
        Guid questionBankId,
        QuestionType questionType,
        string questionText,
        Difficulty difficulty,
        decimal defaultMarks,
        string? questionImageUrl = null,
        string? explanation = null)
        : base(id)
    {
        QuestionBankId = questionBankId;
        QuestionType = questionType;
        QuestionText = questionText;
        Difficulty = difficulty;
        DefaultMarks = defaultMarks;
        QuestionImageUrl = questionImageUrl;
        Explanation = explanation;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new question
    /// </summary>
    public static Result<Question> Create(
        Guid questionBankId,
        QuestionType questionType,
        string questionText,
        Difficulty difficulty,
        decimal defaultMarks,
        string? questionImageUrl = null,
        string? explanation = null)
    {
        if (questionBankId == Guid.Empty)
        {
            return Result<Question>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(questionText))
        {
            return Result<Question>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (defaultMarks <= 0)
        {
            return Result<Question>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (questionImageUrl != null && questionImageUrl.Length > 500)
        {
            return Result<Question>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var question = new Question(
            Guid.NewGuid(),
            questionBankId,
            questionType,
            questionText,
            difficulty,
            defaultMarks,
            questionImageUrl,
            explanation
        );

        return Result<Question>.Success(question);
    }

    /// <summary>
    /// Updates question details
    /// </summary>
    public Result Update(
        string questionText,
        Difficulty difficulty,
        decimal defaultMarks,
        string? questionImageUrl = null,
        string? explanation = null)
    {
        if (string.IsNullOrWhiteSpace(questionText))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (defaultMarks <= 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (questionImageUrl != null && questionImageUrl.Length > 500)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        QuestionText = questionText;
        Difficulty = difficulty;
        DefaultMarks = defaultMarks;
        QuestionImageUrl = questionImageUrl;
        Explanation = explanation;

        return Result.Success();
    }

    /// <summary>
    /// Sets the question image
    /// </summary>
    public Result SetImage(string? questionImageUrl)
    {
        if (questionImageUrl != null && questionImageUrl.Length > 500)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        QuestionImageUrl = questionImageUrl;
        return Result.Success();
    }

    /// <summary>
    /// Sets the explanation
    /// </summary>
    public void SetExplanation(string? explanation) => Explanation = explanation;

    /// <summary>
    /// Deactivates the question
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the question
    /// </summary>
    public void Activate() => IsActive = true;
}
