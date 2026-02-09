using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a student's answer to a specific question in an attempt
/// </summary>
public sealed class StudentAnswer : Entity<Guid>
{
    /// <summary>
    /// Reference to the attempt
    /// </summary>
    public Guid AttemptId { get; private set; }

    /// <summary>
    /// Reference to the question being answered
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Selected option (for MCQ questions)
    /// </summary>
    public Guid? SelectedOptionId { get; private set; }

    /// <summary>
    /// Boolean answer (for True/False questions)
    /// </summary>
    public bool? BooleanAnswer { get; private set; }

    /// <summary>
    /// URL to uploaded file (for file upload questions)
    /// </summary>
    public string? UploadedFileUrl { get; private set; }

    /// <summary>
    /// Original file name (for file upload questions)
    /// </summary>
    public string? UploadedFileName { get; private set; }

    /// <summary>
    /// Whether this answer is correct (auto-graded or manually graded)
    /// </summary>
    public bool? IsCorrect { get; private set; }

    /// <summary>
    /// Score awarded for this answer
    /// </summary>
    public decimal? Score { get; private set; }

    /// <summary>
    /// Feedback/comments on this answer
    /// </summary>
    public string? Feedback { get; private set; }

    /// <summary>
    /// When the answer was provided
    /// </summary>
    public DateTime AnsweredAtUtc { get; private set; }

    // EF Core constructor
    private StudentAnswer() : base()
    {
    }

    private StudentAnswer(
        Guid id,
        Guid attemptId,
        Guid questionId)
        : base(id)
    {
        AttemptId = attemptId;
        QuestionId = questionId;
        AnsweredAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new MCQ answer
    /// </summary>
    public static Result<StudentAnswer> CreateMcqAnswer(
        Guid attemptId,
        Guid questionId,
        Guid selectedOptionId)
    {
        if (attemptId == Guid.Empty)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (questionId == Guid.Empty)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (selectedOptionId == Guid.Empty)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var answer = new StudentAnswer(
            Guid.NewGuid(),
            attemptId,
            questionId
        );

        answer.SelectedOptionId = selectedOptionId;

        return Result<StudentAnswer>.Success(answer);
    }

    /// <summary>
    /// Factory method to create a new True/False answer
    /// </summary>
    public static Result<StudentAnswer> CreateTrueFalseAnswer(
        Guid attemptId,
        Guid questionId,
        bool booleanAnswer)
    {
        if (attemptId == Guid.Empty)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (questionId == Guid.Empty)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var answer = new StudentAnswer(
            Guid.NewGuid(),
            attemptId,
            questionId
        );

        answer.BooleanAnswer = booleanAnswer;

        return Result<StudentAnswer>.Success(answer);
    }

    /// <summary>
    /// Factory method to create a new file upload answer
    /// </summary>
    public static Result<StudentAnswer> CreateFileUploadAnswer(
        Guid attemptId,
        Guid questionId,
        string uploadedFileUrl,
        string uploadedFileName)
    {
        if (attemptId == Guid.Empty)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (questionId == Guid.Empty)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(uploadedFileUrl))
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (uploadedFileUrl.Length > 1000)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (string.IsNullOrWhiteSpace(uploadedFileName))
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (uploadedFileName.Length > 300)
        {
            return Result<StudentAnswer>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var answer = new StudentAnswer(
            Guid.NewGuid(),
            attemptId,
            questionId
        );

        answer.UploadedFileUrl = uploadedFileUrl;
        answer.UploadedFileName = uploadedFileName;

        return Result<StudentAnswer>.Success(answer);
    }

    /// <summary>
    /// Marks the answer as correct/incorrect and assigns a score
    /// </summary>
    public Result Grade(bool isCorrect, decimal score, string? feedback = null)
    {
        if (score < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        IsCorrect = isCorrect;
        Score = score;
        Feedback = feedback;

        return Result.Success();
    }

    /// <summary>
    /// Updates the feedback for this answer
    /// </summary>
    public void UpdateFeedback(string? feedback) => Feedback = feedback;

    /// <summary>
    /// Updates the score for this answer
    /// </summary>
    public Result UpdateScore(decimal score)
    {
        if (score < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Score = score;
        return Result.Success();
    }
}
