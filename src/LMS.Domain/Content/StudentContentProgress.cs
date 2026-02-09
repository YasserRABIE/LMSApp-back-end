using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Tracks student progress for a specific content item
/// </summary>
public sealed class StudentContentProgress : Entity<Guid>
{
    /// <summary>
    /// Reference to the student
    /// </summary>
    public Guid StudentId { get; private set; }

    /// <summary>
    /// Reference to the content item
    /// </summary>
    public Guid ContentItemId { get; private set; }

    /// <summary>
    /// Current progress status
    /// </summary>
    public ProgressStatus Status { get; private set; }

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public decimal ProgressPercent { get; private set; }

    /// <summary>
    /// Last playback position in seconds (for video content)
    /// </summary>
    public int? LastPosition { get; private set; }

    /// <summary>
    /// Total time spent on this content in seconds
    /// </summary>
    public int TimeSpentSeconds { get; private set; }

    /// <summary>
    /// When the student first started this content
    /// </summary>
    public DateTime? StartedAtUtc { get; private set; }

    /// <summary>
    /// When the student completed this content
    /// </summary>
    public DateTime? CompletedAtUtc { get; private set; }

    /// <summary>
    /// Last time the student accessed this content
    /// </summary>
    public DateTime LastAccessedAtUtc { get; private set; }

    // EF Core constructor
    private StudentContentProgress() : base()
    {
    }

    private StudentContentProgress(
        Guid id,
        Guid studentId,
        Guid contentItemId)
        : base(id)
    {
        StudentId = studentId;
        ContentItemId = contentItemId;
        Status = ProgressStatus.NotStarted;
        ProgressPercent = 0;
        TimeSpentSeconds = 0;
        LastAccessedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new progress tracking entry
    /// </summary>
    public static Result<StudentContentProgress> Create(
        Guid studentId,
        Guid contentItemId)
    {
        if (studentId == Guid.Empty)
        {
            return Result<StudentContentProgress>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (contentItemId == Guid.Empty)
        {
            return Result<StudentContentProgress>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var progress = new StudentContentProgress(
            Guid.NewGuid(),
            studentId,
            contentItemId
        );

        return Result<StudentContentProgress>.Success(progress);
    }

    /// <summary>
    /// Marks the content as started
    /// </summary>
    public Result Start()
    {
        if (Status == ProgressStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("PROGRESS.ALREADY_COMPLETED")
            );
        }

        if (Status == ProgressStatus.NotStarted)
        {
            Status = ProgressStatus.InProgress;
            StartedAtUtc = DateTime.UtcNow;
        }

        LastAccessedAtUtc = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Updates progress for the content
    /// </summary>
    public Result UpdateProgress(
        decimal progressPercent,
        int? lastPosition = null,
        int additionalTimeSpentSeconds = 0)
    {
        if (progressPercent < 0 || progressPercent > 100)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (additionalTimeSpentSeconds < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        // Automatically start if not started
        if (Status == ProgressStatus.NotStarted)
        {
            Status = ProgressStatus.InProgress;
            StartedAtUtc = DateTime.UtcNow;
        }

        ProgressPercent = progressPercent;
        LastPosition = lastPosition;
        TimeSpentSeconds += additionalTimeSpentSeconds;
        LastAccessedAtUtc = DateTime.UtcNow;

        // Auto-complete if progress reaches 100%
        if (progressPercent >= 100 && Status != ProgressStatus.Completed)
        {
            Status = ProgressStatus.Completed;
            CompletedAtUtc = DateTime.UtcNow;
        }

        return Result.Success();
    }

    /// <summary>
    /// Marks the content as completed
    /// </summary>
    public Result MarkAsCompleted()
    {
        if (Status == ProgressStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("PROGRESS.ALREADY_COMPLETED")
            );
        }

        Status = ProgressStatus.Completed;
        ProgressPercent = 100;
        CompletedAtUtc = DateTime.UtcNow;
        LastAccessedAtUtc = DateTime.UtcNow;

        if (StartedAtUtc == null)
        {
            StartedAtUtc = DateTime.UtcNow;
        }

        return Result.Success();
    }

    /// <summary>
    /// Updates the last playback position (for video content)
    /// </summary>
    public Result UpdateLastPosition(int positionSeconds)
    {
        if (positionSeconds < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        LastPosition = positionSeconds;
        LastAccessedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Adds time spent on this content
    /// </summary>
    public Result AddTimeSpent(int seconds)
    {
        if (seconds < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        TimeSpentSeconds += seconds;
        LastAccessedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Resets progress (for re-watching or retrying)
    /// </summary>
    public void Reset()
    {
        Status = ProgressStatus.NotStarted;
        ProgressPercent = 0;
        LastPosition = null;
        TimeSpentSeconds = 0;
        StartedAtUtc = null;
        CompletedAtUtc = null;
        LastAccessedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates last accessed time (for tracking engagement)
    /// </summary>
    public void UpdateLastAccessed()
    {
        LastAccessedAtUtc = DateTime.UtcNow;
    }
}
