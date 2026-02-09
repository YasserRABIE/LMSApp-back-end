using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// StudyPlan aggregate root
/// Represents a generated study plan for a student for a specific module
/// </summary>
public sealed class StudyPlan : AggregateRoot<StudyPlanId>
{
    /// <summary>
    /// Reference to the student
    /// </summary>
    public Guid StudentId { get; private set; }

    /// <summary>
    /// Reference to the module
    /// </summary>
    public Guid ModuleId { get; private set; }

    /// <summary>
    /// Reference to the settings used to generate this plan
    /// </summary>
    public Guid SettingsId { get; private set; }

    /// <summary>
    /// Plan start date
    /// </summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// Plan end date
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>
    /// When the module was released (for late purchase calculation)
    /// </summary>
    public DateOnly ModuleReleaseDate { get; private set; }

    /// <summary>
    /// Whether this was a late purchase (purchased after release)
    /// </summary>
    public bool IsLatePurchase { get; private set; }

    /// <summary>
    /// Current status of the plan
    /// </summary>
    public PlanStatus Status { get; private set; }

    /// <summary>
    /// Overall completion percentage (0-100)
    /// </summary>
    public decimal CompletionPercent { get; private set; }

    /// <summary>
    /// Total number of tasks in the plan
    /// </summary>
    public short TotalPlannedTasks { get; private set; }

    /// <summary>
    /// Number of completed tasks
    /// </summary>
    public short CompletedTasks { get; private set; }

    /// <summary>
    /// Number of missed tasks
    /// </summary>
    public short MissedTasks { get; private set; }

    /// <summary>
    /// Number of times the plan was rescheduled
    /// </summary>
    public short RescheduledCount { get; private set; }

    /// <summary>
    /// When the plan was generated
    /// </summary>
    public DateTime GeneratedAtUtc { get; private set; }

    /// <summary>
    /// When the plan was last adjusted
    /// </summary>
    public DateTime? LastAdjustedAtUtc { get; private set; }

    // EF Core constructor
    private StudyPlan() : base(StudyPlanId.New())
    {
    }

    private StudyPlan(
        StudyPlanId id,
        Guid studentId,
        Guid moduleId,
        Guid settingsId,
        DateOnly startDate,
        DateOnly endDate,
        DateOnly moduleReleaseDate,
        bool isLatePurchase,
        short totalPlannedTasks)
        : base(id)
    {
        StudentId = studentId;
        ModuleId = moduleId;
        SettingsId = settingsId;
        StartDate = startDate;
        EndDate = endDate;
        ModuleReleaseDate = moduleReleaseDate;
        IsLatePurchase = isLatePurchase;
        Status = PlanStatus.Active;
        CompletionPercent = 0;
        TotalPlannedTasks = totalPlannedTasks;
        CompletedTasks = 0;
        MissedTasks = 0;
        RescheduledCount = 0;
        GeneratedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new study plan
    /// </summary>
    public static Result<StudyPlan> Create(
        Guid studentId,
        Guid moduleId,
        Guid settingsId,
        DateOnly startDate,
        DateOnly endDate,
        DateOnly moduleReleaseDate,
        bool isLatePurchase,
        short totalPlannedTasks)
    {
        if (studentId == Guid.Empty)
        {
            return Result<StudyPlan>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (moduleId == Guid.Empty)
        {
            return Result<StudyPlan>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (settingsId == Guid.Empty)
        {
            return Result<StudyPlan>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (endDate < startDate)
        {
            return Result<StudyPlan>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (totalPlannedTasks < 0)
        {
            return Result<StudyPlan>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var plan = new StudyPlan(
            StudyPlanId.New(),
            studentId,
            moduleId,
            settingsId,
            startDate,
            endDate,
            moduleReleaseDate,
            isLatePurchase,
            totalPlannedTasks
        );

        return Result<StudyPlan>.Success(plan);
    }

    /// <summary>
    /// Updates progress statistics
    /// </summary>
    public Result UpdateProgress(short completedTasks, short missedTasks)
    {
        if (completedTasks < 0 || missedTasks < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        CompletedTasks = completedTasks;
        MissedTasks = missedTasks;
        CompletionPercent = TotalPlannedTasks > 0
            ? (decimal)completedTasks / TotalPlannedTasks * 100
            : 0;

        // Auto-complete if all tasks are done
        if (CompletedTasks == TotalPlannedTasks)
        {
            Status = PlanStatus.Completed;
        }

        return Result.Success();
    }

    /// <summary>
    /// Marks the plan as adjusted/rescheduled
    /// </summary>
    public void MarkAsAdjusted()
    {
        RescheduledCount++;
        LastAdjustedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Abandons the study plan
    /// </summary>
    public Result Abandon()
    {
        if (Status == PlanStatus.Completed)
        {
            return Result.Failure(
                Error.Conflict("PLAN.ALREADY_COMPLETED")
            );
        }

        Status = PlanStatus.Abandoned;
        return Result.Success();
    }

    /// <summary>
    /// Marks the plan as completed
    /// </summary>
    public Result Complete()
    {
        if (Status == PlanStatus.Abandoned)
        {
            return Result.Failure(
                Error.Conflict("PLAN.ABANDONED")
            );
        }

        Status = PlanStatus.Completed;
        CompletionPercent = 100;
        return Result.Success();
    }
}
