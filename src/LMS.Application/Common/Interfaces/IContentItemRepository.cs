using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface IContentItemRepository : IRepository<ContentItem, ContentItemId>
{
    Task<IReadOnlyList<ContentItem>> GetContentItemsByStageAsync(StageId stageId, CancellationToken cancellationToken = default);
    Task<VideoContent?> GetVideoContentAsync(ContentItemId contentItemId, CancellationToken cancellationToken = default);
    Task<FileContent?> GetFileContentAsync(ContentItemId contentItemId, CancellationToken cancellationToken = default);
    Task<int> GetMaxDisplayOrderAsync(StageId stageId, CancellationToken cancellationToken = default);
}
