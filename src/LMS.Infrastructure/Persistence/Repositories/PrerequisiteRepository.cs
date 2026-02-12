using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class PrerequisiteRepository : Repository<Prerequisite, PrerequisiteId>, IPrerequisiteRepository
{
    public PrerequisiteRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Prerequisite>> GetByTargetEntityAsync(TargetEntityType targetEntityType,
        string targetEntityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.TargetEntityType == targetEntityType && p.TargetEntityId == targetEntityId)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Prerequisite>> GetActivePrerequisitesByTargetAsync(
        TargetEntityType targetEntityType, string targetEntityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.TargetEntityType == targetEntityType &&
                        p.TargetEntityId == targetEntityId &&
                        p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}