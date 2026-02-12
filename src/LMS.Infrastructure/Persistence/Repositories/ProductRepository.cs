using LMS.Application.Common.Interfaces;
using LMS.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product, ProductId>, IProductRepository
{
    public ProductRepository(LmsDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByReferenceAsync(ProductType productType, Guid referenceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.ProductType == productType && p.ReferenceId == referenceId, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsByTypeAsync(ProductType productType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ProductType == productType)
            .ToListAsync(cancellationToken);
    }
}
