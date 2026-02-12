using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class ContentItemRepository : Repository<ContentItem, ContentItemId>, IContentItemRepository
{
    public ContentItemRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ContentItem>> GetContentItemsByStageAsync(StageId stageId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.StageId == stageId)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<VideoContent?> GetVideoContentAsync(ContentItemId contentItemId, CancellationToken cancellationToken = default)
    {
        return await _context.VideoContents
            .FirstOrDefaultAsync(v => v.ContentItemId == contentItemId, cancellationToken);
    }

    public async Task<FileContent?> GetFileContentAsync(ContentItemId contentItemId, CancellationToken cancellationToken = default)
    {
        return await _context.FileContents
            .FirstOrDefaultAsync(f => f.ContentItemId == contentItemId, cancellationToken);
    }

    public async Task<int> GetMaxDisplayOrderAsync(StageId stageId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.StageId == stageId)
            .MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0;
    }
}
