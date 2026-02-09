using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a score for a specific rubric criterion in an attempt
/// Used for manual grading with rubrics
/// </summary>
public sealed class AttemptRubricScore : Entity<Guid>
{
    /// <summary>
    /// Reference to the attempt being graded
    /// </summary>
    public Guid AttemptId { get; private set; }

    /// <summary>
    /// Reference to the rubric criterion
    /// </summary>
    public Guid CriteriaId { get; private set; }

    /// <summary>
    /// Score awarded for this criterion
    /// </summary>
    public decimal Score { get; private set; }

    /// <summary>
    /// Grader's notes/justification for this score
    /// </summary>
    public string? Notes { get; private set; }

    // EF Core constructor
    private AttemptRubricScore() : base()
    {
    }

    private AttemptRubricScore(
        Guid id,
        Guid attemptId,
        Guid criteriaId,
        decimal score,
        string? notes = null)
        : base(id)
    {
        AttemptId = attemptId;
        CriteriaId = criteriaId;
        Score = score;
        Notes = notes;
    }

    /// <summary>
    /// Factory method to create a new rubric score
    /// </summary>
    public static Result<AttemptRubricScore> Create(
        Guid attemptId,
        Guid criteriaId,
        decimal score,
        string? notes = null)
    {
        if (attemptId == Guid.Empty)
        {
            return Result<AttemptRubricScore>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (criteriaId == Guid.Empty)
        {
            return Result<AttemptRubricScore>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (score < 0)
        {
            return Result<AttemptRubricScore>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var rubricScore = new AttemptRubricScore(
            Guid.NewGuid(),
            attemptId,
            criteriaId,
            score,
            notes
        );

        return Result<AttemptRubricScore>.Success(rubricScore);
    }

    /// <summary>
    /// Updates the score and notes
    /// </summary>
    public Result Update(decimal score, string? notes = null)
    {
        if (score < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Score = score;
        Notes = notes;

        return Result.Success();
    }

    /// <summary>
    /// Updates just the notes
    /// </summary>
    public void UpdateNotes(string? notes) => Notes = notes;
}
