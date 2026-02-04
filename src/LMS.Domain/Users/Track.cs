using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Represents an academic track (e.g., Scientific, Literary)
/// Reference data - typically seeded in database
/// </summary>
public sealed class Track : Entity<Guid>
{
    /// <summary>
    /// Track name in Arabic
    /// </summary>
    public string NameAr { get; private set; }

    /// <summary>
    /// Track name in English
    /// </summary>
    public string NameEn { get; private set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Indicates if this track is active
    /// </summary>
    public bool IsActive { get; private set; }

    // Navigation properties
    // public ICollection<StudyLevelTrack> StudyLevelTracks { get; private set; } = new List<StudyLevelTrack>();

    // EF Core constructor
    private Track() : base()
    {
        NameAr = string.Empty;
        NameEn = string.Empty;
    }

    private Track(
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
    /// Factory method to create a new track
    /// </summary>
    public static Result<Track> Create(
        string nameAr,
        string nameEn,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
        {
            return Result<Track>.Failure(
                ErrorCodes.Validation.Required,
                "Arabic name is required",
                ErrorType.Validation
            );
        }

        if (string.IsNullOrWhiteSpace(nameEn))
        {
            return Result<Track>.Failure(
                ErrorCodes.Validation.Required,
                "English name is required",
                ErrorType.Validation
            );
        }

        if (displayOrder < 0)
        {
            return Result<Track>.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Display order cannot be negative",
                ErrorType.Validation
            );
        }

        var track = new Track(
            Guid.NewGuid(),
            nameAr,
            nameEn,
            displayOrder
        );

        return Result<Track>.Success(track);
    }

    /// <summary>
    /// Deactivates the track
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the track
    /// </summary>
    public void Activate() => IsActive = true;
}
