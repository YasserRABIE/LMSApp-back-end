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
    /// User's first name
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// User's second name (middle name)
    /// </summary>
    public string SecondName { get; private set; }

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; private set; }

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
    /// Optional email for notifications only (not for authentication)
    /// </summary>
    public string? Email { get; private set; }

    // Navigation properties (loaded separately)
    // These will be populated by EF Core

    /// <summary>
    /// Gets the full name by concatenating first, second, and last names
    /// </summary>
    public string GetFullName() => $"{FirstName} {SecondName} {LastName}".Trim();

    // EF Core constructor
    private User() : base()
    {
        Phone = null!;
        PasswordHash = string.Empty;
        FirstName = string.Empty;
        SecondName = string.Empty;
        LastName = string.Empty;
    }

    private User(
        UserId id,
        Phone phone,
        string passwordHash,
        string firstName,
        string secondName,
        string lastName,
        UserType userType,
        bool isPhoneVerified)
        : base(id)
    {
        Phone = phone;
        PasswordHash = passwordHash;
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        UserType = userType;
        IsPhoneVerified = isPhoneVerified;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new user (Student/Teacher/Assistant)
    /// </summary>
    public static Result<User> Create(
        Phone phone,
        string passwordHash,
        string firstName,
        string secondName,
        string lastName,
        UserType userType,
        bool isPhoneVerified = false)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(secondName))
        {
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (firstName.Length > 100)
        {
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (secondName.Length > 100)
        {
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (lastName.Length > 100)
        {
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var user = new User(
            UserId.New(),
            phone,
            passwordHash,
            firstName,
            secondName,
            lastName,
            userType,
            isPhoneVerified
        );

        // Raise domain event
        user.RaiseDomainEvent(new UserRegisteredEvent(
            user.Id,
            user.Phone,
            user.UserType,
            user.FirstName,
            user.SecondName,
            user.LastName
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
            "Parent", // Default first name, will be updated on first login
            "",
            "",
            UserType.Parent,
            isPhoneVerified: true // Already verified during student link process
        );

        user.RaiseDomainEvent(new UserRegisteredEvent(
            user.Id,
            user.Phone,
            user.UserType,
            user.FirstName,
            user.SecondName,
            user.LastName
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
                Error.Validation(ErrorCodes.User.AlreadyVerified)
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
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        PasswordHash = newPasswordHash;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Updates user's profile information
    /// </summary>
    public Result UpdateProfile(string firstName, string secondName, string lastName, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(secondName))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (firstName.Length > 100)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (secondName.Length > 100)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (lastName.Length > 100)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        Email = email;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new UserProfileUpdatedEvent(Id, FirstName, SecondName, LastName));

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
                Error.Validation(ErrorCodes.User.Inactive)
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
                Error.Validation(ErrorCodes.Validation.InvalidInput)
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
                Error.Forbidden(ErrorCodes.User.Inactive)
            );
        }

        if (!IsPhoneVerified)
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.User.NotVerified)
            );
        }

        return Result.Success();
    }
}
