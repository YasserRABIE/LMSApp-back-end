using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Represents a student's study plan preferences for a specific module
/// </summary>
public sealed class StudyPlanSettings : Entity<Guid>
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
    /// Daily study time in minutes
    /// </summary>
    public short DailyStudyMinutes { get; private set; }

    /// <summary>
    /// Preferred start time for daily study
    /// </summary>
    public TimeSpan? PreferredStartTime { get; private set; }

    /// <summary>
    /// Whether to include review days in the plan
    /// </summary>
    public bool IncludeReviewDays { get; private set; }

    // EF Core constructor
    private StudyPlanSettings() : base()
    {
    }

    private StudyPlanSettings(
        Guid id,
        Guid studentId,
        Guid moduleId,
        short dailyStudyMinutes,
        TimeSpan? preferredStartTime = null,
        bool includeReviewDays = true)
        : base(id)
    {
        StudentId = studentId;
        ModuleId = moduleId;
        DailyStudyMinutes = dailyStudyMinutes;
        PreferredStartTime = preferredStartTime;
        IncludeReviewDays = includeReviewDays;
    }

    /// <summary>
    /// Factory method to create new study plan settings
    /// </summary>
    public static Result<StudyPlanSettings> Create(
        Guid studentId,
        Guid moduleId,
        short dailyStudyMinutes,
        TimeSpan? preferredStartTime = null,
        bool includeReviewDays = true)
    {
        if (studentId == Guid.Empty)
        {
            return Result<StudyPlanSettings>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (moduleId == Guid.Empty)
        {
            return Result<StudyPlanSettings>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (dailyStudyMinutes < 30 || dailyStudyMinutes > 480)
        {
            return Result<StudyPlanSettings>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var settings = new StudyPlanSettings(
            Guid.NewGuid(),
            studentId,
            moduleId,
            dailyStudyMinutes,
            preferredStartTime,
            includeReviewDays
        );

        return Result<StudyPlanSettings>.Success(settings);
    }

    /// <summary>
    /// Updates study plan settings
    /// </summary>
    public Result Update(
        short dailyStudyMinutes,
        TimeSpan? preferredStartTime = null,
        bool includeReviewDays = true)
    {
        if (dailyStudyMinutes < 30 || dailyStudyMinutes > 480)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        DailyStudyMinutes = dailyStudyMinutes;
        PreferredStartTime = preferredStartTime;
        IncludeReviewDays = includeReviewDays;

        return Result.Success();
    }
}
