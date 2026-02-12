using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface ICourseCategoryRepository : IRepository<CourseCategory, CourseCategoryId>
{
    Task<IReadOnlyList<CourseCategory>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
}
