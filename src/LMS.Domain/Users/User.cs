using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// User aggregate root - represents a user in the system
/// Authentication is phone-based only (no email)
/// </summary>
public sealed class User : AggregateRoot<UserId>
{
    /// <summary>
    /// User's phone number (Egyptian format: 01XXXXXXXXX)
    /// </summary>
    public Phone Phone { get; private set; }

    /// <summary>
    /// Hashed password for authentication
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Type of user (Student, Teacher, Assistant, Parent, Admin)
    /// </summary>
    public UserType UserType { get; private set; }

    /// <summary>
    /// Indicates if the user's phone number has been verified
    /// </summary>
    public bool IsPhoneVerified { get; private set; }

    /// <summary>
    /// Indicates if the user account is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// When the user was created
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// When the user was last updated
    /// </summary>
    public DateTime? UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Optional email for notifications only (not for authentication)
    /// </summary>
    public string? Email { get; private set; }

    // Navigation properties (loaded separately)
    // These will be populated by EF Core

    // EF Core constructor
    private User() : base()
    {
        Phone = null!;
        PasswordHash = string.Empty;
        FullName = string.Empty;
    }

    private User(
        UserId id,
        Phone phone,
        string passwordHash,
        string fullName,
        UserType userType,
        bool isPhoneVerified)
        : base(id)
    {
        Phone = phone;
        PasswordHash = passwordHash;
        FullName = fullName;
        UserType = userType;
        IsPhoneVerified = isPhoneVerified;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new user (Student/Teacher/Assistant)
    /// </summary>
    public static Result<User> Create(
        Phone phone,
        string passwordHash,
        string fullName,
        UserType userType,
        bool isPhoneVerified = false)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result<User>.Failure(
                ErrorCodes.Validation.Required,
                "Password hash is required",
                ErrorType.Validation
            );
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result<User>.Failure(
                ErrorCodes.Validation.Required,
                "Full name is required",
                ErrorType.Validation
            );
        }

        if (fullName.Length > 200)
        {
            return Result<User>.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Full name cannot exceed 200 characters",
                ErrorType.Validation
            );
        }

        var user = new User(
            UserId.New(),
            phone,
            passwordHash,
            fullName,
            userType,
            isPhoneVerified
        );

        // Raise domain event
        user.RaiseDomainEvent(new UserRegisteredEvent(
            user.Id,
            user.Phone,
            user.UserType,
            user.FullName
        ));

        return Result<User>.Success(user);
    }

    /// <summary>
    /// Factory method to create a parent user (student-initiated)
    /// Password is set to phone number temporarily
    /// </summary>
    public static Result<User> CreateParent(
        Phone phone,
        string temporaryPasswordHash)
    {
        var user = new User(
            UserId.New(),
            phone,
            temporaryPasswordHash,
            "Parent", // Default name, will be updated on first login
            UserType.Parent,
            isPhoneVerified: true // Already verified during student link process
        );

        user.RaiseDomainEvent(new UserRegisteredEvent(
            user.Id,
            user.Phone,
            user.UserType,
            user.FullName
        ));

        return Result<User>.Success(user);
    }

    /// <summary>
    /// Verifies the user's phone number
    /// </summary>
    public Result VerifyPhone()
    {
        if (IsPhoneVerified)
        {
            return Result.Failure(
                ErrorCodes.User.AlreadyVerified,
                "Phone number is already verified",
                ErrorType.Validation
            );
        }

        IsPhoneVerified = true;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new UserPhoneVerifiedEvent(Id, Phone));

        return Result.Success();
    }

    /// <summary>
    /// Updates user's password
    /// </summary>
    public Result UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            return Result.Failure(
                ErrorCodes.Validation.Required,
                "Password hash is required",
                ErrorType.Validation
            );
        }

        PasswordHash = newPasswordHash;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Updates user's profile information
    /// </summary>
    public Result UpdateProfile(string fullName, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Failure(
                ErrorCodes.Validation.Required,
                "Full name is required",
                ErrorType.Validation
            );
        }

        if (fullName.Length > 200)
        {
            return Result.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Full name cannot exceed 200 characters",
                ErrorType.Validation
            );
        }

        FullName = fullName;
        Email = email;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new UserProfileUpdatedEvent(Id, FullName));

        return Result.Success();
    }

    /// <summary>
    /// Deactivates the user account
    /// </summary>
    public Result Deactivate(string reason)
    {
        if (!IsActive)
        {
            return Result.Failure(
                ErrorCodes.User.Inactive,
                "User is already inactive",
                ErrorType.Validation
            );
        }

        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new UserDeactivatedEvent(Id, reason));

        return Result.Success();
    }

    /// <summary>
    /// Reactivates the user account
    /// </summary>
    public Result Reactivate()
    {
        if (IsActive)
        {
            return Result.Failure(
                ErrorCodes.Validation.InvalidInput,
                "User is already active",
                ErrorType.Validation
            );
        }

        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Checks if user can login
    /// </summary>
    public Result CanLogin()
    {
        if (!IsActive)
        {
            return Result.Failure(
                ErrorCodes.User.Inactive,
                "User account is inactive",
                ErrorType.Forbidden
            );
        }

        if (!IsPhoneVerified)
        {
            return Result.Failure(
                ErrorCodes.User.NotVerified,
                "Phone number is not verified",
                ErrorType.Forbidden
            );
        }

        return Result.Success();
    }
}
