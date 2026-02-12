using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface IStageRepository : IRepository<Stage, StageId>
{
    Task<IReadOnlyList<Stage>> GetStagesByModuleAsync(ModuleId moduleId, CancellationToken cancellationToken = default);
    Task<int> GetMaxDisplayOrderAsync(ModuleId moduleId, CancellationToken cancellationToken = default);
}
