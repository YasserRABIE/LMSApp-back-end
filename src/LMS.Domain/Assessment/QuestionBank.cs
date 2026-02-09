using LMS.Domain.Common;
using LMS.Domain.Content;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a collection of questions for a specific course
/// </summary>
public sealed class QuestionBank : Entity<Guid>
{
    /// <summary>
    /// Reference to the course this question bank belongs to
    /// </summary>
    public CourseId CourseId { get; private set; }

    /// <summary>
    /// Question bank name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Question bank description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Indicates if this question bank is active
    /// </summary>
    public bool IsActive { get; private set; }

    // EF Core constructor
    private QuestionBank() : base()
    {
        Name = string.Empty;
        CourseId = CourseId.New();
    }

    private QuestionBank(
        Guid id,
        CourseId courseId,
        string name,
        string? description)
        : base(id)
    {
        CourseId = courseId;
        Name = name;
        Description = description;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new question bank
    /// </summary>
    public static Result<QuestionBank> Create(
        CourseId courseId,
        string name,
        string? description = null)
    {
        if (courseId.Value == Guid.Empty)
        {
            return Result<QuestionBank>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<QuestionBank>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (name.Length > 200)
        {
            return Result<QuestionBank>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var questionBank = new QuestionBank(
            Guid.NewGuid(),
            courseId,
            name,
            description
        );

        return Result<QuestionBank>.Success(questionBank);
    }

    /// <summary>
    /// Updates question bank details
    /// </summary>
    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (name.Length > 200)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Name = name;
        Description = description;

        return Result.Success();
    }

    /// <summary>
    /// Deactivates the question bank
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the question bank
    /// </summary>
    public void Activate() => IsActive = true;
}
