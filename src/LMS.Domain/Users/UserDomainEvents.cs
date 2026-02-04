using LMS.Domain.Common;

namespace LMS.Domain.Users;

/// <summary>
/// Domain event raised when a new user is registered
/// </summary>
public sealed record UserRegisteredEvent(
    UserId UserId,
    string Phone,
    UserType UserType,
    string FullName
) : DomainEvent;

/// <summary>
/// Domain event raised when a user's phone number is verified
/// </summary>
public sealed record UserPhoneVerifiedEvent(
    UserId UserId,
    string Phone
) : DomainEvent;

/// <summary>
/// Domain event raised when a user is deactivated
/// </summary>
public sealed record UserDeactivatedEvent(
    UserId UserId,
    string Reason
) : DomainEvent;

/// <summary>
/// Domain event raised when a user's profile is updated
/// </summary>
public sealed record UserProfileUpdatedEvent(
    UserId UserId,
    string FullName
) : DomainEvent;
