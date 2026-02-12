using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class CourseRepository : Repository<Course, CourseId>, ICourseRepository
{
    public CourseRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Course>> GetCoursesByTeacherAsync(UserId teacherId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.TeacherId == teacherId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetCoursesBySubjectAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.SubjectId == subjectId && c.IsActive)
            .OrderBy(c => c.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Visibility == Visibility.Published && c.IsActive && c.PublishedAtUtc != null)
            .OrderByDescending(c => c.PublishedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetCoursesByStudyLevelAndTrackAsync(Guid studyLevelId, Guid trackId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.StudyLevelId == studyLevelId && c.TrackId == trackId && c.IsActive && c.Visibility == Visibility.Published)
            .OrderBy(c => c.Title)
            .ToListAsync(cancellationToken);
    }
}
