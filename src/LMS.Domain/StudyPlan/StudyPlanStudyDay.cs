using LMS.Domain.Common;

namespace LMS.Domain.StudyPlan;

/// <summary>
/// Represents a day of the week when a student will study
/// </summary>
public sealed class StudyPlanStudyDay : Entity<Guid>
{
    /// <summary>
    /// Reference to the study plan settings
    /// </summary>
    public Guid SettingsId { get; private set; }

    /// <summary>
    /// Day of the week (0=Sunday, 1=Monday, ..., 6=Saturday)
    /// </summary>
    public short DayOfWeek { get; private set; }

    // EF Core constructor
    private StudyPlanStudyDay() : base()
    {
    }

    private StudyPlanStudyDay(
        Guid id,
        Guid settingsId,
        short dayOfWeek)
        : base(id)
    {
        SettingsId = settingsId;
        DayOfWeek = dayOfWeek;
    }

    /// <summary>
    /// Factory method to create a study day
    /// </summary>
    public static Result<StudyPlanStudyDay> Create(
        Guid settingsId,
        short dayOfWeek)
    {
        if (settingsId == Guid.Empty)
        {
            return Result<StudyPlanStudyDay>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (dayOfWeek < 0 || dayOfWeek > 6)
        {
            return Result<StudyPlanStudyDay>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var studyDay = new StudyPlanStudyDay(
            Guid.NewGuid(),
            settingsId,
            dayOfWeek
        );

        return Result<StudyPlanStudyDay>.Success(studyDay);
    }
}
