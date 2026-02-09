using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents an option/choice for MCQ and True/False questions
/// </summary>
public sealed class QuestionOption : Entity<Guid>
{
    /// <summary>
    /// Reference to the question
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Option text
    /// </summary>
    public string OptionText { get; private set; }

    /// <summary>
    /// Optional image URL for the option
    /// </summary>
    public string? OptionImageUrl { get; private set; }

    /// <summary>
    /// Indicates if this is the correct answer
    /// </summary>
    public bool IsCorrect { get; private set; }

    /// <summary>
    /// Display order for the option
    /// </summary>
    public short DisplayOrder { get; private set; }

    // EF Core constructor
    private QuestionOption() : base()
    {
        OptionText = string.Empty;
    }

    private QuestionOption(
        Guid id,
        Guid questionId,
        string optionText,
        bool isCorrect,
        short displayOrder,
        string? optionImageUrl = null)
        : base(id)
    {
        QuestionId = questionId;
        OptionText = optionText;
        IsCorrect = isCorrect;
        DisplayOrder = displayOrder;
        OptionImageUrl = optionImageUrl;
    }

    /// <summary>
    /// Factory method to create a new question option
    /// </summary>
    public static Result<QuestionOption> Create(
        Guid questionId,
        string optionText,
        bool isCorrect,
        short displayOrder,
        string? optionImageUrl = null)
    {
        if (questionId == Guid.Empty)
        {
            return Result<QuestionOption>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(optionText))
        {
            return Result<QuestionOption>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (optionImageUrl != null && optionImageUrl.Length > 500)
        {
            return Result<QuestionOption>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var option = new QuestionOption(
            Guid.NewGuid(),
            questionId,
            optionText,
            isCorrect,
            displayOrder,
            optionImageUrl
        );

        return Result<QuestionOption>.Success(option);
    }

    /// <summary>
    /// Updates option details
    /// </summary>
    public Result Update(
        string optionText,
        bool isCorrect,
        short displayOrder,
        string? optionImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(optionText))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (optionImageUrl != null && optionImageUrl.Length > 500)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        OptionText = optionText;
        IsCorrect = isCorrect;
        DisplayOrder = displayOrder;
        OptionImageUrl = optionImageUrl;

        return Result.Success();
    }

    /// <summary>
    /// Marks this option as correct
    /// </summary>
    public void MarkAsCorrect() => IsCorrect = true;

    /// <summary>
    /// Marks this option as incorrect
    /// </summary>
    public void MarkAsIncorrect() => IsCorrect = false;

    /// <summary>
    /// Sets the option image
    /// </summary>
    public Result SetImage(string? optionImageUrl)
    {
        if (optionImageUrl != null && optionImageUrl.Length > 500)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        OptionImageUrl = optionImageUrl;
        return Result.Success();
    }
}
