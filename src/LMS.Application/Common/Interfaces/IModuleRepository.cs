using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface IModuleRepository : IRepository<Module, ModuleId>
{
    Task<IReadOnlyList<Module>> GetModulesByCourseAsync(CourseId courseId, CancellationToken cancellationToken = default);
    Task<int> GetMaxDisplayOrderAsync(CourseId courseId, CancellationToken cancellationToken = default);
}
