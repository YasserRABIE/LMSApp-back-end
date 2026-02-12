using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class ModuleRepository : Repository<Module, ModuleId>, IModuleRepository
{
    public ModuleRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Module>> GetModulesByCourseAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMaxDisplayOrderAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.CourseId == courseId)
            .MaxAsync(m => (int?)m.DisplayOrder, cancellationToken) ?? 0;
    }
}
