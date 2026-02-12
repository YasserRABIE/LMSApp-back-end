using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class CourseCategoryRepository : Repository<CourseCategory, CourseCategoryId>, ICourseCategoryRepository
{
    public CourseCategoryRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<CourseCategory>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(cc => cc.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
