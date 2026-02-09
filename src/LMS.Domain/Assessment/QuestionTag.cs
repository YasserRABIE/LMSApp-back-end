using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a tag/label for categorizing questions
/// </summary>
public sealed class QuestionTag : Entity<Guid>
{
    /// <summary>
    /// Reference to the question
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Tag name/label
    /// </summary>
    public string Tag { get; private set; }

    // EF Core constructor
    private QuestionTag() : base()
    {
        Tag = string.Empty;
    }

    private QuestionTag(
        Guid id,
        Guid questionId,
        string tag)
        : base(id)
    {
        QuestionId = questionId;
        Tag = tag;
    }

    /// <summary>
    /// Factory method to create a new question tag
    /// </summary>
    public static Result<QuestionTag> Create(
        Guid questionId,
        string tag)
    {
        if (questionId == Guid.Empty)
        {
            return Result<QuestionTag>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(tag))
        {
            return Result<QuestionTag>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (tag.Length > 50)
        {
            return Result<QuestionTag>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var questionTag = new QuestionTag(
            Guid.NewGuid(),
            questionId,
            tag.Trim()
        );

        return Result<QuestionTag>.Success(questionTag);
    }
}
