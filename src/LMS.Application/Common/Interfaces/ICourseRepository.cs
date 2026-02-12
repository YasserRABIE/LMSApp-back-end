using LMS.Domain.Content;
using LMS.Domain.Users;

namespace LMS.Application.Common.Interfaces;

public interface ICourseRepository : IRepository<Course, CourseId>
{
    Task<IReadOnlyList<Course>> GetCoursesByTeacherAsync(UserId teacherId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetCoursesBySubjectAsync(SubjectId subjectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetCoursesByStudyLevelAndTrackAsync(Guid studyLevelId, Guid trackId, CancellationToken cancellationToken = default);
}
