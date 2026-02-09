using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Profile information specific to students
/// </summary>
public sealed class StudentProfile : Entity<Guid>
{
    /// <summary>
    /// Reference to the user
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Student's study level and track combination ID
    /// </summary>
    public Guid StudyLevelTrackId { get; private set; }

    /// <summary>
    /// Student's school name (optional)
    /// </summary>
    public string? SchoolName { get; private set; }

    /// <summary>
    /// Student's governorate/city
    /// </summary>
    public string? Governorate { get; private set; }

    // Navigation properties
    // public User User { get; private set; } = null!;
    // public StudyLevelTrack StudyLevelTrack { get; private set; } = null!;

    // EF Core constructor
    private StudentProfile() : base()
    {
        UserId = null!;
    }

    private StudentProfile(
        Guid id,
        UserId userId,
        Guid studyLevelTrackId,
        string? schoolName,
        string? governorate)
        : base(id)
    {
        UserId = userId;
        StudyLevelTrackId = studyLevelTrackId;
        SchoolName = schoolName;
        Governorate = governorate;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new student profile
    /// </summary>
    public static Result<StudentProfile> Create(
        UserId userId,
        Guid studyLevelTrackId,
        string? schoolName = null,
        string? governorate = null)
    {
        if (studyLevelTrackId == Guid.Empty)
        {
            return Result<StudentProfile>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (!string.IsNullOrWhiteSpace(schoolName) && schoolName.Length > 200)
        {
            return Result<StudentProfile>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (!string.IsNullOrWhiteSpace(governorate) && governorate.Length > 100)
        {
            return Result<StudentProfile>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var profile = new StudentProfile(
            Guid.NewGuid(),
            userId,
            studyLevelTrackId,
            schoolName,
            governorate
        );

        return Result<StudentProfile>.Success(profile);
    }

    /// <summary>
    /// Updates the student profile information
    /// </summary>
    public Result UpdateProfile(
        Guid? studyLevelTrackId = null,
        string? schoolName = null,
        string? governorate = null)
    {
        if (studyLevelTrackId.HasValue && studyLevelTrackId.Value != Guid.Empty)
        {
            StudyLevelTrackId = studyLevelTrackId.Value;
        }

        if (schoolName != null)
        {
            if (schoolName.Length > 200)
            {
                return Result.Failure(
                    Error.Validation(ErrorCodes.Validation.InvalidInput)
                );
            }
            SchoolName = schoolName;
        }

        if (governorate != null)
        {
            if (governorate.Length > 100)
            {
                return Result.Failure(
                    Error.Validation(ErrorCodes.Validation.InvalidInput)
                );
            }
            Governorate = governorate;
        }

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
