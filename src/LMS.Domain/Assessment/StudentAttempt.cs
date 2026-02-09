using LMS.Domain.Common;

namespace LMS.Domain.Assessment;

/// <summary>
/// Represents a student's attempt at an assessment
/// </summary>
public sealed class StudentAttempt : Entity<Guid>
{
    /// <summary>
    /// Reference to the student
    /// </summary>
    public Guid StudentId { get; private set; }

    /// <summary>
    /// Reference to the assessment
    /// </summary>
    public AssessmentId AssessmentId { get; private set; }

    /// <summary>
    /// Attempt number (1, 2, 3, etc.)
    /// </summary>
    public short AttemptNumber { get; private set; }

    /// <summary>
    /// Current status of the attempt
    /// </summary>
    public AttemptStatus Status { get; private set; }

    /// <summary>
    /// When the attempt was started
    /// </summary>
    public DateTime StartedAtUtc { get; private set; }

    /// <summary>
    /// When the attempt was submitted
    /// </summary>
    public DateTime? SubmittedAtUtc { get; private set; }

    /// <summary>
    /// When the attempt is due (based on duration limit)
    /// </summary>
    public DateTime? DueAtUtc { get; private set; }

    /// <summary>
    /// Whether this was a late submission
    /// </summary>
    public bool IsLateSubmission { get; private set; }

    /// <summary>
    /// Total score achieved
    /// </summary>
    public decimal? TotalScore { get; private set; }

    /// <summary>
    /// Percentage score (0-100)
    /// </summary>
    public decimal? PercentageScore { get; private set; }

    /// <summary>
    /// Whether the student passed
    /// </summary>
    public bool? IsPassed { get; private set; }

    /// <summary>
    /// Reference to the assistant who graded this attempt
    /// </summary>
    public Guid? GradedByAssistantId { get; private set; }

    /// <summary>
    /// When the attempt was graded
    /// </summary>
    public DateTime? GradedAtUtc { get; private set; }

    /// <summary>
    /// XP points awarded for this attempt
    /// </summary>
    public short XpAwarded { get; private set; }

    /// <summary>
    /// Game points awarded for this attempt
    /// </summary>
    public short PointsAwarded { get; private set; }

    /// <summary>
    /// Grader's notes/feedback
    /// </summary>
    public string? GraderNotes { get; private set; }

    // EF Core constructor
    private StudentAttempt() : base()
    {
        AssessmentId = AssessmentId.New();
    }

    private StudentAttempt(
        Guid id,
        Guid studentId,
        AssessmentId assessmentId,
        short attemptNumber,
        DateTime? dueAtUtc = null)
        : base(id)
    {
        StudentId = studentId;
        AssessmentId = assessmentId;
        AttemptNumber = attemptNumber;
        Status = AttemptStatus.InProgress;
        StartedAtUtc = DateTime.UtcNow;
        DueAtUtc = dueAtUtc;
        IsLateSubmission = false;
        XpAwarded = 0;
        PointsAwarded = 0;
    }

    /// <summary>
    /// Factory method to create a new student attempt
    /// </summary>
    public static Result<StudentAttempt> Create(
        Guid studentId,
        AssessmentId assessmentId,
        short attemptNumber,
        DateTime? dueAtUtc = null)
    {
        if (studentId == Guid.Empty)
        {
            return Result<StudentAttempt>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (assessmentId.Value == Guid.Empty)
        {
            return Result<StudentAttempt>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (attemptNumber < 1)
        {
            return Result<StudentAttempt>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var attempt = new StudentAttempt(
            Guid.NewGuid(),
            studentId,
            assessmentId,
            attemptNumber,
            dueAtUtc
        );

        return Result<StudentAttempt>.Success(attempt);
    }

    /// <summary>
    /// Submits the attempt
    /// </summary>
    public Result Submit(bool isLate = false)
    {
        if (Status != AttemptStatus.InProgress)
        {
            return Result.Failure(
                Error.Conflict("ATTEMPT.INVALID_STATUS")
            );
        }

        Status = AttemptStatus.Submitted;
        SubmittedAtUtc = DateTime.UtcNow;
        IsLateSubmission = isLate;

        return Result.Success();
    }

    /// <summary>
    /// Marks the attempt as timed out
    /// </summary>
    public Result TimeOut()
    {
        if (Status != AttemptStatus.InProgress)
        {
            return Result.Failure(
                Error.Conflict("ATTEMPT.INVALID_STATUS")
            );
        }

        Status = AttemptStatus.TimedOut;
        SubmittedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Marks the attempt as abandoned
    /// </summary>
    public Result Abandon()
    {
        if (Status != AttemptStatus.InProgress)
        {
            return Result.Failure(
                Error.Conflict("ATTEMPT.INVALID_STATUS")
            );
        }

        Status = AttemptStatus.Abandoned;
        return Result.Success();
    }

    /// <summary>
    /// Grades the attempt with a score
    /// </summary>
    public Result Grade(
        decimal totalScore,
        decimal totalMarks,
        decimal passingMarks,
        Guid? gradedByAssistantId = null,
        string? graderNotes = null)
    {
        if (Status != AttemptStatus.Submitted && Status != AttemptStatus.TimedOut)
        {
            return Result.Failure(
                Error.Conflict("ATTEMPT.INVALID_STATUS")
            );
        }

        if (totalScore < 0 || totalScore > totalMarks)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        TotalScore = totalScore;
        PercentageScore = totalMarks > 0 ? (totalScore / totalMarks) * 100 : 0;
        IsPassed = totalScore >= passingMarks;
        GradedByAssistantId = gradedByAssistantId;
        GradedAtUtc = DateTime.UtcNow;
        GraderNotes = graderNotes;
        Status = AttemptStatus.Graded;

        return Result.Success();
    }

    /// <summary>
    /// Awards gamification points for this attempt
    /// </summary>
    public Result AwardPoints(short xpAwarded, short pointsAwarded)
    {
        if (xpAwarded < 0 || pointsAwarded < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        XpAwarded = xpAwarded;
        PointsAwarded = pointsAwarded;

        return Result.Success();
    }

    /// <summary>
    /// Updates grader notes
    /// </summary>
    public void UpdateGraderNotes(string? graderNotes) => GraderNotes = graderNotes;
}
