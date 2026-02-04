using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Profile information specific to assistants (follow-up supporters)
/// </summary>
public sealed class AssistantProfile : Entity<Guid>
{
    /// <summary>
    /// Reference to the user
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Assistant's biography/description
    /// </summary>
    public string? Bio { get; private set; }

    /// <summary>
    /// Maximum number of students the assistant can handle
    /// </summary>
    public int MaxStudents { get; private set; }

    /// <summary>
    /// Current number of active students assigned to this assistant
    /// </summary>
    public int CurrentStudentCount { get; private set; }

    /// <summary>
    /// Average rating from students (0-5)
    /// </summary>
    public decimal AverageRating { get; private set; }

    /// <summary>
    /// Total number of ratings received
    /// </summary>
    public int TotalRatings { get; private set; }

    /// <summary>
    /// When the profile was created
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// When the profile was last updated
    /// </summary>
    public DateTime? UpdatedAtUtc { get; private set; }

    // Navigation properties
    // public User User { get; private set; } = null!;

    // EF Core constructor
    private AssistantProfile() : base()
    {
        UserId = null!;
    }

    private AssistantProfile(
        Guid id,
        UserId userId,
        string? bio,
        int maxStudents)
        : base(id)
    {
        UserId = userId;
        Bio = bio;
        MaxStudents = maxStudents;
        CurrentStudentCount = 0;
        AverageRating = 0;
        TotalRatings = 0;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new assistant profile
    /// </summary>
    public static Result<AssistantProfile> Create(
        UserId userId,
        int maxStudents = 50,
        string? bio = null)
    {
        if (maxStudents <= 0)
        {
            return Result<AssistantProfile>.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Maximum students must be greater than zero",
                ErrorType.Validation
            );
        }

        if (!string.IsNullOrWhiteSpace(bio) && bio.Length > 2000)
        {
            return Result<AssistantProfile>.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Bio cannot exceed 2000 characters",
                ErrorType.Validation
            );
        }

        var profile = new AssistantProfile(
            Guid.NewGuid(),
            userId,
            bio,
            maxStudents
        );

        return Result<AssistantProfile>.Success(profile);
    }

    /// <summary>
    /// Updates the assistant profile information
    /// </summary>
    public Result UpdateProfile(string? bio = null, int? maxStudents = null)
    {
        if (bio != null)
        {
            if (bio.Length > 2000)
            {
                return Result.Failure(
                    ErrorCodes.Validation.InvalidInput,
                    "Bio cannot exceed 2000 characters",
                    ErrorType.Validation
                );
            }
            Bio = bio;
        }

        if (maxStudents.HasValue)
        {
            if (maxStudents.Value <= 0)
            {
                return Result.Failure(
                    ErrorCodes.Validation.InvalidInput,
                    "Maximum students must be greater than zero",
                    ErrorType.Validation
                );
            }

            if (maxStudents.Value < CurrentStudentCount)
            {
                return Result.Failure(
                    ErrorCodes.Validation.InvalidInput,
                    $"Cannot set maximum students below current student count ({CurrentStudentCount})",
                    ErrorType.Validation
                );
            }

            MaxStudents = maxStudents.Value;
        }

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Checks if the assistant can accept more students
    /// </summary>
    public bool CanAcceptMoreStudents() => CurrentStudentCount < MaxStudents;

    /// <summary>
    /// Increments the current student count
    /// </summary>
    public Result IncrementStudentCount()
    {
        if (!CanAcceptMoreStudents())
        {
            return Result.Failure(
                ErrorCodes.FollowUp.GroupFull,
                "Assistant has reached maximum student capacity",
                ErrorType.Conflict
            );
        }

        CurrentStudentCount++;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Decrements the current student count
    /// </summary>
    public Result DecrementStudentCount()
    {
        if (CurrentStudentCount <= 0)
        {
            return Result.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Current student count is already zero",
                ErrorType.Validation
            );
        }

        CurrentStudentCount--;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Updates the rating based on a new rating
    /// </summary>
    public Result AddRating(int rating)
    {
        if (rating < 1 || rating > 5)
        {
            return Result.Failure(
                ErrorCodes.Validation.OutOfRange,
                "Rating must be between 1 and 5",
                ErrorType.Validation
            );
        }

        // Calculate new average
        decimal totalScore = AverageRating * TotalRatings + rating;
        TotalRatings++;
        AverageRating = totalScore / TotalRatings;

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
