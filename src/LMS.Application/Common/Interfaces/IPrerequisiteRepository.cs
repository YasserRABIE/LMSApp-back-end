using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface IPrerequisiteRepository : IRepository<Prerequisite, PrerequisiteId>
{
    Task<IReadOnlyList<Prerequisite>> GetByTargetEntityAsync(TargetEntityType targetEntityType, string targetEntityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prerequisite>> GetActivePrerequisitesByTargetAsync(TargetEntityType targetEntityType, string targetEntityId, CancellationToken cancellationToken = default);
}
