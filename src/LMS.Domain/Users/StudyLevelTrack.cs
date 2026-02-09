using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Represents a valid combination of study level and track
/// Example: 3rd Secondary + Scientific Track
/// Reference data - typically seeded in database
/// </summary>
public sealed class StudyLevelTrack : Entity<Guid>
{
    /// <summary>
    /// Reference to the study level
    /// </summary>
    public Guid StudyLevelId { get; private set; }

    /// <summary>
    /// Reference to the track
    /// </summary>
    public Guid TrackId { get; private set; }

    /// <summary>
    /// Indicates if this combination is active
    /// </summary>
    public bool IsActive { get; private set; }

    // Navigation properties
    // public StudyLevel StudyLevel { get; private set; } = null!;
    // public Track Track { get; private set; } = null!;

    // EF Core constructor
    private StudyLevelTrack() : base()
    {
    }

    private StudyLevelTrack(
        Guid id,
        Guid studyLevelId,
        Guid trackId)
        : base(id)
    {
        StudyLevelId = studyLevelId;
        TrackId = trackId;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new study level-track combination
    /// </summary>
    public static Result<StudyLevelTrack> Create(
        Guid studyLevelId,
        Guid trackId)
    {
        if (studyLevelId == Guid.Empty)
        {
            return Result<StudyLevelTrack>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (trackId == Guid.Empty)
        {
            return Result<StudyLevelTrack>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var studyLevelTrack = new StudyLevelTrack(
            Guid.NewGuid(),
            studyLevelId,
            trackId
        );

        return Result<StudyLevelTrack>.Success(studyLevelTrack);
    }

    /// <summary>
    /// Deactivates the study level-track combination
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the study level-track combination
    /// </summary>
    public void Activate() => IsActive = true;
}
