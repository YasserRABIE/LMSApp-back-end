using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class SchoolTypeRepository : Repository<SchoolType, SchoolTypeId>, ISchoolTypeRepository
{
    public SchoolTypeRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<SchoolType>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(st => st.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
