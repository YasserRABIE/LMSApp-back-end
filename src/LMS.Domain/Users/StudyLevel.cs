using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Represents an education level (e.g., 1st Secondary, 2nd Secondary, 3rd Secondary)
/// Reference data - typically seeded in database
/// </summary>
public sealed class StudyLevel : Entity<Guid>
{
    /// <summary>
    /// Level name in Arabic
    /// </summary>
    public string NameAr { get; private set; }

    /// <summary>
    /// Level name in English
    /// </summary>
    public string NameEn { get; private set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Indicates if this level is active
    /// </summary>
    public bool IsActive { get; private set; }

    // Navigation properties
    // public ICollection<StudyLevelTrack> StudyLevelTracks { get; private set; } = new List<StudyLevelTrack>();

    // EF Core constructor
    private StudyLevel() : base()
    {
        NameAr = string.Empty;
        NameEn = string.Empty;
    }

    private StudyLevel(
        Guid id,
        string nameAr,
        string nameEn,
        int displayOrder)
        : base(id)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new study level
    /// </summary>
    public static Result<StudyLevel> Create(
        string nameAr,
        string nameEn,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
        {
            return Result<StudyLevel>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(nameEn))
        {
            return Result<StudyLevel>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (displayOrder < 0)
        {
            return Result<StudyLevel>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var level = new StudyLevel(
            Guid.NewGuid(),
            nameAr,
            nameEn,
            displayOrder
        );

        return Result<StudyLevel>.Success(level);
    }

    /// <summary>
    /// Deactivates the study level
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the study level
    /// </summary>
    public void Activate() => IsActive = true;
}
