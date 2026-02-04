namespace LMS.Domain.Common;

/// <summary>
/// Base class for aggregate roots (entities that are entry points to aggregates)
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Collection of domain events raised by this aggregate
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Parameterless constructor for EF Core
    /// </summary>
    protected AggregateRoot() : base()
    {
    }

    /// <summary>
    /// Adds a domain event to be dispatched
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events (typically called after they've been dispatched)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}

/// <summary>
/// Base class for domain events
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
