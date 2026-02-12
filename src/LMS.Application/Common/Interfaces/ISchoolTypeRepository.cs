using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface ISchoolTypeRepository : IRepository<SchoolType, SchoolTypeId>
{
    Task<IReadOnlyList<SchoolType>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
}
