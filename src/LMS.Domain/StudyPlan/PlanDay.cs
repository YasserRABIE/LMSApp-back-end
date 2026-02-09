using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Represents a single day in a study plan
/// </summary>
public sealed class PlanDay : Entity<Guid>
{
    /// <summary>
    /// Reference to the study plan
    /// </summary>
    public StudyPlanId StudyPlanId { get; private set; }

    /// <summary>
    /// The date for this plan day
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// Type of day (Study, Review, Rest, FollowUp)
    /// </summary>
    public DayType DayType { get; private set; }

    /// <summary>
    /// Current status of this day
    /// </summary>
    public DayStatus Status { get; private set; }

    /// <summary>
    /// Planned study time in minutes
    /// </summary>
    public short PlannedMinutes { get; private set; }

    /// <summary>
    /// Actual time spent in minutes
    /// </summary>
    public short? ActualMinutes { get; private set; }

    /// <summary>
    /// Optional notes for this day
    /// </summary>
    public string? Notes { get; private set; }

    // EF Core constructor
    private PlanDay() : base()
    {
        StudyPlanId = StudyPlanId.New();
    }

    private PlanDay(
        Guid id,
        StudyPlanId studyPlanId,
        DateOnly date,
        DayType dayType,
        short plannedMinutes)
        : base(id)
    {
        StudyPlanId = studyPlanId;
        Date = date;
        DayType = dayType;
        PlannedMinutes = plannedMinutes;
        Status = DayStatus.Upcoming;
    }

    /// <summary>
    /// Factory method to create a plan day
    /// </summary>
    public static Result<PlanDay> Create(
        StudyPlanId studyPlanId,
        DateOnly date,
        DayType dayType,
        short plannedMinutes)
    {
        if (studyPlanId.Value == Guid.Empty)
        {
            return Result<PlanDay>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (plannedMinutes < 0)
        {
            return Result<PlanDay>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var planDay = new PlanDay(
            Guid.NewGuid(),
            studyPlanId,
            date,
            dayType,
            plannedMinutes
        );

        return Result<PlanDay>.Success(planDay);
    }

    /// <summary>
    /// Marks the day as completed with actual time spent
    /// </summary>
    public Result Complete(short actualMinutes)
    {
        if (Status == DayStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("DAY.ALREADY_COMPLETED")
            );
        }

        if (actualMinutes < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Status = DayStatus.Completed;
        ActualMinutes = actualMinutes;

        return Result.Success();
    }

    /// <summary>
    /// Marks the day as missed
    /// </summary>
    public Result MarkAsMissed()
    {
        if (Status == DayStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("DAY.ALREADY_COMPLETED")
            );
        }

        Status = DayStatus.Missed;
        return Result.Success();
    }

    /// <summary>
    /// Marks the day as rescheduled
    /// </summary>
    public Result Reschedule()
    {
        if (Status == DayStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("DAY.ALREADY_COMPLETED")
            );
        }

        Status = DayStatus.Rescheduled;
        return Result.Success();
    }

    /// <summary>
    /// Updates notes for this day
    /// </summary>
    public Result UpdateNotes(string? notes)
    {
        if (notes != null && notes.Length > 500)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Notes = notes;
        return Result.Success();
    }
}
