using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Types of plan adjustments
/// </summary>
public enum AdjustmentType : byte
{
    MissedDay = 1,
    LowScore = 2,
    FollowUpFeedback = 3,
    Manual = 4,
    Reschedule = 5
}

/// <summary>
/// Logs adjustments made to a study plan
/// </summary>
public sealed class PlanAdjustmentLog : Entity<Guid>
{
    /// <summary>
    /// Reference to the study plan
    /// </summary>
    public StudyPlanId StudyPlanId { get; private set; }

    /// <summary>
    /// Type of adjustment
    /// </summary>
    public AdjustmentType AdjustmentType { get; private set; }

    /// <summary>
    /// Reason for the adjustment
    /// </summary>
    public string Reason { get; private set; }

    /// <summary>
    /// User who made the adjustment (null for automated)
    /// </summary>
    public Guid? AdjustedByUserId { get; private set; }

    /// <summary>
    /// Number of tasks rescheduled
    /// </summary>
    public short TasksRescheduled { get; private set; }

    /// <summary>
    /// Number of review tasks added
    /// </summary>
    public short ReviewTasksAdded { get; private set; }

    /// <summary>
    /// Previous end date before adjustment
    /// </summary>
    public DateOnly? PreviousEndDate { get; private set; }

    /// <summary>
    /// New end date after adjustment
    /// </summary>
    public DateOnly? NewEndDate { get; private set; }

    // EF Core constructor
    private PlanAdjustmentLog() : base()
    {
        Reason = string.Empty;
        StudyPlanId = StudyPlanId.New();
    }

    private PlanAdjustmentLog(
        Guid id,
        StudyPlanId studyPlanId,
        AdjustmentType adjustmentType,
        string reason,
        Guid? adjustedByUserId = null,
        short tasksRescheduled = 0,
        short reviewTasksAdded = 0,
        DateOnly? previousEndDate = null,
        DateOnly? newEndDate = null)
        : base(id)
    {
        StudyPlanId = studyPlanId;
        AdjustmentType = adjustmentType;
        Reason = reason;
        AdjustedByUserId = adjustedByUserId;
        TasksRescheduled = tasksRescheduled;
        ReviewTasksAdded = reviewTasksAdded;
        PreviousEndDate = previousEndDate;
        NewEndDate = newEndDate;
    }

    /// <summary>
    /// Factory method to create an adjustment log
    /// </summary>
    public static Result<PlanAdjustmentLog> Create(
        StudyPlanId studyPlanId,
        AdjustmentType adjustmentType,
        string reason,
        Guid? adjustedByUserId = null,
        short tasksRescheduled = 0,
        short reviewTasksAdded = 0,
        DateOnly? previousEndDate = null,
        DateOnly? newEndDate = null)
    {
        if (studyPlanId.Value == Guid.Empty)
        {
            return Result<PlanAdjustmentLog>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result<PlanAdjustmentLog>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (reason.Length > 500)
        {
            return Result<PlanAdjustmentLog>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var log = new PlanAdjustmentLog(
            Guid.NewGuid(),
            studyPlanId,
            adjustmentType,
            reason,
            adjustedByUserId,
            tasksRescheduled,
            reviewTasksAdded,
            previousEndDate,
            newEndDate
        );

        return Result<PlanAdjustmentLog>.Success(log);
    }
}
