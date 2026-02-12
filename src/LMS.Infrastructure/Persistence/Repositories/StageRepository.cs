using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class StageRepository : Repository<Stage, StageId>, IStageRepository
{
    public StageRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Stage>> GetStagesByModuleAsync(ModuleId moduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ModuleId == moduleId)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMaxDisplayOrderAsync(ModuleId moduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ModuleId == moduleId)
            .MaxAsync(s => (int?)s.DisplayOrder, cancellationToken) ?? 0;
    }
}
