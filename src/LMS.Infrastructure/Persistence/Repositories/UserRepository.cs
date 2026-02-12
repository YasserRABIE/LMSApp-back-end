using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for User aggregate
/// </summary>
public sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByPhoneAsync(
        Phone phone,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Phone.Value == phone.Value, cancellationToken);
    }

    public async Task<User?> GetByPhoneWithProfileAsync(
        Phone phone,
        UserType userType,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Phone.Value == phone.Value);

        query = userType switch
        {
            UserType.Student => query.Include("StudentProfile"),
            UserType.Teacher => query.Include("TeacherProfile"),
            UserType.Assistant => query.Include("AssistantProfile"),
            UserType.Parent => query.Include("ParentProfile"),
            _ => query
        };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByIdWithProfileAsync(
        UserId userId,
        UserType userType,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Id == userId);

        query = userType switch
        {
            UserType.Student => query.Include("StudentProfile"),
            UserType.Teacher => query.Include("TeacherProfile"),
            UserType.Assistant => query.Include("AssistantProfile"),
            UserType.Parent => query.Include("ParentProfile"),
            _ => query
        };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetWithActiveSessionsAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => _context.DeviceSessions.Where(ds => ds.UserId == userId && ds.IsActive))
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(
        Phone phone,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Phone.Value == phone.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<DeviceSession>> GetActiveSessionsAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DeviceSessions
            .Where(ds => ds.UserId == userId && ds.IsActive)
            .OrderByDescending(ds => ds.LastAccessedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeviceSession?> GetSessionByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await _context.DeviceSessions
            .FirstOrDefaultAsync(ds => ds.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task<DeviceSession?> GetSessionByIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DeviceSessions
            .FindAsync(new object[] { sessionId }, cancellationToken);
    }

    public async Task AddSessionAsync(
        DeviceSession session,
        CancellationToken cancellationToken = default)
    {
        await _context.DeviceSessions.AddAsync(session, cancellationToken);
    }

    public void UpdateSession(DeviceSession session)
    {
        _context.DeviceSessions.Update(session);
    }

    public async Task RevokeAllSessionsAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        var sessions = await _context.DeviceSessions
            .Where(ds => ds.UserId == userId && ds.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }
    }

    public async Task AddStudentProfileAsync(
        StudentProfile profile,
        CancellationToken cancellationToken = default)
    {
        await _context.StudentProfiles.AddAsync(profile, cancellationToken);
    }

    public async Task AddTeacherProfileAsync(
        TeacherProfile profile,
        CancellationToken cancellationToken = default)
    {
        await _context.TeacherProfiles.AddAsync(profile, cancellationToken);
    }
}
