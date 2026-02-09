using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a single grading criterion within a rubric
/// </summary>
public sealed class RubricCriteria : Entity<Guid>
{
    /// <summary>
    /// Reference to the grading rubric
    /// </summary>
    public Guid RubricId { get; private set; }

    /// <summary>
    /// Criteria name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Criteria description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Maximum score for this criterion
    /// </summary>
    public decimal MaxScore { get; private set; }

    /// <summary>
    /// Display order within the rubric
    /// </summary>
    public short DisplayOrder { get; private set; }

    // EF Core constructor
    private RubricCriteria() : base()
    {
        Name = string.Empty;
    }

    private RubricCriteria(
        Guid id,
        Guid rubricId,
        string name,
        decimal maxScore,
        short displayOrder,
        string? description = null)
        : base(id)
    {
        RubricId = rubricId;
        Name = name;
        MaxScore = maxScore;
        DisplayOrder = displayOrder;
        Description = description;
    }

    /// <summary>
    /// Factory method to create a new rubric criterion
    /// </summary>
    public static Result<RubricCriteria> Create(
        Guid rubricId,
        string name,
        decimal maxScore,
        short displayOrder,
        string? description = null)
    {
        if (rubricId == Guid.Empty)
        {
            return Result<RubricCriteria>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<RubricCriteria>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (name.Length > 200)
        {
            return Result<RubricCriteria>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (maxScore <= 0)
        {
            return Result<RubricCriteria>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var criteria = new RubricCriteria(
            Guid.NewGuid(),
            rubricId,
            name,
            maxScore,
            displayOrder,
            description
        );

        return Result<RubricCriteria>.Success(criteria);
    }

    /// <summary>
    /// Updates criteria details
    /// </summary>
    public Result Update(
        string name,
        decimal maxScore,
        short displayOrder,
        string? description = null)
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

        if (maxScore <= 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Name = name;
        MaxScore = maxScore;
        DisplayOrder = displayOrder;
        Description = description;

        return Result.Success();
    }
}
