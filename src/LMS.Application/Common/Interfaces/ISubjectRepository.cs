using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Subject entity
/// </summary>
public interface ISubjectRepository : IRepository<Subject, SubjectId>
{
    /// <summary>
    /// Gets all active subjects ordered by display order
    /// </summary>
    Task<IReadOnlyList<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets core subjects ordered by display order
    /// </summary>
    Task<IReadOnlyList<Subject>> GetCoreSubjectsAsync(CancellationToken cancellationToken = default);
}
