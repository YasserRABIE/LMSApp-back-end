using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface ITagRepository : IRepository<Tag, TagId>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
}
