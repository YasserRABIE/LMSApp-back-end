using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class SubjectRepository : Repository<Subject, SubjectId>, ISubjectRepository
{
    public SubjectRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Subject>> GetCoreSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsCore && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
