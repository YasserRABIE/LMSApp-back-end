using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Profile information specific to teachers
/// </summary>
public sealed class TeacherProfile : Entity<Guid>
{
    /// <summary>
    /// Reference to the user
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Teacher's biography/description
    /// </summary>
    public string? Bio { get; private set; }

    /// <summary>
    /// Teacher's specialization/subject expertise
    /// </summary>
    public string? Specialization { get; private set; }

    /// <summary>
    /// Years of experience
    /// </summary>
    public int? YearsOfExperience { get; private set; }

    /// <summary>
    /// Teacher's qualifications
    /// </summary>
    public string? Qualifications { get; private set; }

    /// <summary>
    /// Social media links (JSON or comma-separated)
    /// </summary>
    public string? SocialLinks { get; private set; }

    /// <summary>
    /// Profile image URL
    /// </summary>
    public string? ProfileImageUrl { get; private set; }

    // Navigation properties
    // public User User { get; private set; } = null!;

    // EF Core constructor
    private TeacherProfile() : base()
    {
        UserId = null!;
    }

    private TeacherProfile(
        Guid id,
        UserId userId,
        string? bio,
        string? specialization,
        int? yearsOfExperience,
        string? qualifications)
        : base(id)
    {
        UserId = userId;
        Bio = bio;
        Specialization = specialization;
        YearsOfExperience = yearsOfExperience;
        Qualifications = qualifications;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new teacher profile
    /// </summary>
    public static Result<TeacherProfile> Create(
        UserId userId,
        string? bio = null,
        string? specialization = null,
        int? yearsOfExperience = null,
        string? qualifications = null)
    {
        if (!string.IsNullOrWhiteSpace(bio) && bio.Length > 2000)
        {
            return Result<TeacherProfile>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (!string.IsNullOrWhiteSpace(specialization) && specialization.Length > 200)
        {
            return Result<TeacherProfile>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (yearsOfExperience.HasValue && yearsOfExperience.Value < 0)
        {
            return Result<TeacherProfile>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var profile = new TeacherProfile(
            Guid.NewGuid(),
            userId,
            bio,
            specialization,
            yearsOfExperience,
            qualifications
        );

        return Result<TeacherProfile>.Success(profile);
    }

    /// <summary>
    /// Updates the teacher profile information
    /// </summary>
    public Result UpdateProfile(
        string? bio = null,
        string? specialization = null,
        int? yearsOfExperience = null,
        string? qualifications = null,
        string? socialLinks = null,
        string? profileImageUrl = null)
    {
        if (bio != null)
        {
            if (bio.Length > 2000)
            {
                return Result.Failure(
                    Error.Validation(ErrorCodes.Validation.InvalidInput)
                );
            }
            Bio = bio;
        }

        if (specialization != null)
        {
            if (specialization.Length > 200)
            {
                return Result.Failure(
                    Error.Validation(ErrorCodes.Validation.InvalidInput)
                );
            }
            Specialization = specialization;
        }

        if (yearsOfExperience.HasValue)
        {
            if (yearsOfExperience.Value < 0)
            {
                return Result.Failure(
                    Error.Validation(ErrorCodes.Validation.InvalidInput)
                );
            }
            YearsOfExperience = yearsOfExperience;
        }

        if (qualifications != null)
        {
            Qualifications = qualifications;
        }

        if (socialLinks != null)
        {
            SocialLinks = socialLinks;
        }

        if (profileImageUrl != null)
        {
            ProfileImageUrl = profileImageUrl;
        }

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
