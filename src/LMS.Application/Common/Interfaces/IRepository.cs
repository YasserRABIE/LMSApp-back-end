using LMS.Domain.Common;
using System.Linq.Expressions;

namespace LMS.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface for data access
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The entity's identifier type</typeparam>
public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching a predicate
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single entity matching a predicate
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Updates multiple entities
    /// </summary>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Removes an entity
    /// </summary>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes multiple entities
    /// </summary>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Checks if any entity matches the predicate
    /// </summary>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}
