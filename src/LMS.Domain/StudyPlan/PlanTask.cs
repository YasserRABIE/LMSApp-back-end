using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Represents a task to complete on a specific plan day
/// </summary>
public sealed class PlanTask : Entity<Guid>
{
    /// <summary>
    /// Reference to the plan day
    /// </summary>
    public Guid PlanDayId { get; private set; }

    /// <summary>
    /// Reference to content item (if this is a content task)
    /// </summary>
    public Guid? ContentItemId { get; private set; }

    /// <summary>
    /// Type of task
    /// </summary>
    public TaskType TaskType { get; private set; }

    /// <summary>
    /// Task title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Task description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Estimated time to complete in minutes
    /// </summary>
    public short EstimatedMinutes { get; private set; }

    /// <summary>
    /// Display order on the day
    /// </summary>
    public short DisplayOrder { get; private set; }

    /// <summary>
    /// Current status of the task
    /// </summary>
    public TaskStatus Status { get; private set; }

    /// <summary>
    /// When the task was completed
    /// </summary>
    public DateTime? CompletedAtUtc { get; private set; }

    /// <summary>
    /// Actual time taken in minutes
    /// </summary>
    public short? ActualMinutes { get; private set; }

    // EF Core constructor
    private PlanTask() : base()
    {
        Title = string.Empty;
    }

    private PlanTask(
        Guid id,
        Guid planDayId,
        TaskType taskType,
        string title,
        short estimatedMinutes,
        short displayOrder,
        Guid? contentItemId = null,
        string? description = null)
        : base(id)
    {
        PlanDayId = planDayId;
        ContentItemId = contentItemId;
        TaskType = taskType;
        Title = title;
        Description = description;
        EstimatedMinutes = estimatedMinutes;
        DisplayOrder = displayOrder;
        Status = TaskStatus.Todo;
    }

    /// <summary>
    /// Factory method to create a plan task
    /// </summary>
    public static Result<PlanTask> Create(
        Guid planDayId,
        TaskType taskType,
        string title,
        short estimatedMinutes,
        short displayOrder,
        Guid? contentItemId = null,
        string? description = null)
    {
        if (planDayId == Guid.Empty)
        {
            return Result<PlanTask>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<PlanTask>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result<PlanTask>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (estimatedMinutes < 0)
        {
            return Result<PlanTask>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var task = new PlanTask(
            Guid.NewGuid(),
            planDayId,
            taskType,
            title,
            estimatedMinutes,
            displayOrder,
            contentItemId,
            description
        );

        return Result<PlanTask>.Success(task);
    }

    /// <summary>
    /// Starts the task
    /// </summary>
    public Result Start()
    {
        if (Status == TaskStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("TASK.ALREADY_COMPLETED")
            );
        }

        Status = TaskStatus.InProgress;
        return Result.Success();
    }

    /// <summary>
    /// Completes the task
    /// </summary>
    public Result Complete(short? actualMinutes = null)
    {
        if (Status == TaskStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("TASK.ALREADY_COMPLETED")
            );
        }

        if (actualMinutes.HasValue && actualMinutes.Value < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Status = TaskStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        ActualMinutes = actualMinutes;

        return Result.Success();
    }

    /// <summary>
    /// Marks task as missed
    /// </summary>
    public Result MarkAsMissed()
    {
        if (Status == TaskStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("TASK.ALREADY_COMPLETED")
            );
        }

        Status = TaskStatus.Missed;
        return Result.Success();
    }

    /// <summary>
    /// Skips the task
    /// </summary>
    public Result Skip()
    {
        if (Status == TaskStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("TASK.ALREADY_COMPLETED")
            );
        }

        Status = TaskStatus.Skipped;
        return Result.Success();
    }
}
