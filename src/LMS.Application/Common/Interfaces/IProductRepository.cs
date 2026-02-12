using LMS.Domain.Purchasing;

namespace LMS.Application.Common.Interfaces;

public interface IProductRepository : IRepository<Product, ProductId>
{
    Task<Product?> GetByReferenceAsync(ProductType productType, Guid referenceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetProductsByTypeAsync(ProductType productType, CancellationToken cancellationToken = default);
}
