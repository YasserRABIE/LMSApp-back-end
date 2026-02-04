using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Profile information specific to parents
/// </summary>
public sealed class ParentProfile : Entity<Guid>
{
    /// <summary>
    /// Reference to the user
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Relationship type to the student (Mother, Father, Guardian)
    /// </summary>
    public ParentRelationType RelationType { get; private set; }

    /// <summary>
    /// Indicates if this is the parent's first login
    /// On first login, they must update their profile and change password
    /// </summary>
    public bool IsFirstLogin { get; private set; }

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
    // public ICollection<ParentStudentLink> LinkedStudents { get; private set; } = new List<ParentStudentLink>();

    // EF Core constructor
    private ParentProfile() : base()
    {
        UserId = null!;
    }

    private ParentProfile(
        Guid id,
        UserId userId,
        ParentRelationType relationType,
        bool isFirstLogin)
        : base(id)
    {
        UserId = userId;
        RelationType = relationType;
        IsFirstLogin = isFirstLogin;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new parent profile
    /// </summary>
    public static Result<ParentProfile> Create(
        UserId userId,
        ParentRelationType relationType = ParentRelationType.Guardian,
        bool isFirstLogin = true)
    {
        var profile = new ParentProfile(
            Guid.NewGuid(),
            userId,
            relationType,
            isFirstLogin
        );

        return Result<ParentProfile>.Success(profile);
    }

    /// <summary>
    /// Marks the first login as completed (profile has been updated)
    /// </summary>
    public Result CompleteFirstLogin()
    {
        if (!IsFirstLogin)
        {
            return Result.Failure(
                ErrorCodes.Validation.InvalidInput,
                "First login has already been completed",
                ErrorType.Validation
            );
        }

        IsFirstLogin = false;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Updates the parent's relationship type
    /// </summary>
    public Result UpdateRelationType(ParentRelationType relationType)
    {
        RelationType = relationType;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Checks if parent must complete first login setup
    /// </summary>
    public bool MustCompleteSetup() => IsFirstLogin;
}

/// <summary>
/// Represents the type of relationship between parent and student
/// </summary>
public enum ParentRelationType
{
    /// <summary>
    /// Mother
    /// </summary>
    Mother = 1,

    /// <summary>
    /// Father
    /// </summary>
    Father = 2,

    /// <summary>
    /// Legal guardian or other relationship
    /// </summary>
    Guardian = 3
}
