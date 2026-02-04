using LMS.Domain.Common;
using LMS.Domain.Users;

namespace LMS.Application.Common.Interfaces;

/// <summary>
/// Repository interface for User aggregate
/// </summary>
public interface IUserRepository : IRepository<User, UserId>
{
    /// <summary>
    /// Gets a user by phone number
    /// </summary>
    Task<User?> GetByPhoneAsync(Phone phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by phone number with their profile included
    /// </summary>
    Task<User?> GetByPhoneWithProfileAsync(
        Phone phone,
        UserType userType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by ID with their profile included
    /// </summary>
    Task<User?> GetByIdWithProfileAsync(
        UserId userId,
        UserType userType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user with their active device sessions
    /// </summary>
    Task<User?> GetWithActiveSessionsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a phone number already exists
    /// </summary>
    Task<bool> PhoneExistsAsync(Phone phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active device sessions for a user
    /// </summary>
    Task<IReadOnlyList<DeviceSession>> GetActiveSessionsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a device session by refresh token
    /// </summary>
    Task<DeviceSession?> GetSessionByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a device session by ID
    /// </summary>
    Task<DeviceSession?> GetSessionByIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a device session
    /// </summary>
    Task AddSessionAsync(DeviceSession session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a device session
    /// </summary>
    void UpdateSession(DeviceSession session);

    /// <summary>
    /// Revokes all active sessions for a user
    /// </summary>
    Task RevokeAllSessionsAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a student profile
    /// </summary>
    Task AddStudentProfileAsync(StudentProfile profile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a teacher profile
    /// </summary>
    Task AddTeacherProfileAsync(TeacherProfile profile, CancellationToken cancellationToken = default);
}
