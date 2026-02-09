using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a grading rubric with multiple criteria
/// Used for manual grading of assignments and file uploads
/// </summary>
public sealed class GradingRubric : Entity<Guid>
{
    /// <summary>
    /// Rubric name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Rubric description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Indicates if this is the default rubric
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// Indicates if this rubric is active
    /// </summary>
    public bool IsActive { get; private set; }

    // EF Core constructor
    private GradingRubric() : base()
    {
        Name = string.Empty;
    }

    private GradingRubric(
        Guid id,
        string name,
        string? description,
        bool isDefault = false)
        : base(id)
    {
        Name = name;
        Description = description;
        IsDefault = isDefault;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new grading rubric
    /// </summary>
    public static Result<GradingRubric> Create(
        string name,
        string? description = null,
        bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<GradingRubric>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (name.Length > 200)
        {
            return Result<GradingRubric>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var rubric = new GradingRubric(
            Guid.NewGuid(),
            name,
            description,
            isDefault
        );

        return Result<GradingRubric>.Success(rubric);
    }

    /// <summary>
    /// Updates rubric details
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
    /// Marks this rubric as the default
    /// </summary>
    public void SetAsDefault() => IsDefault = true;

    /// <summary>
    /// Removes default status from this rubric
    /// </summary>
    public void RemoveDefault() => IsDefault = false;

    /// <summary>
    /// Deactivates the rubric
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the rubric
    /// </summary>
    public void Activate() => IsActive = true;
}
