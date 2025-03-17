namespace Portfolio.Domain.Abstractions;

/// <summary>
/// Serves as a base class for domain entities, providing a unique identifier
/// and mechanisms for handling domain events.
/// </summary>
/// <typeparam name="TEntityId">
/// The type representing the unique identifier of the entity.
/// </typeparam>
/// <remarks>
/// This abstract class establishes a foundation for domain entities by
/// encapsulating shared functionality, such as managing domain events
/// and ensuring the presence of a unique identifier.
/// </remarks>
public abstract class Entity<TEntityId>: IEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    protected Entity(TEntityId id)
    {
        Id = id;
    }
    protected Entity() {}
    public TEntityId Id { get; init; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Triggers a domain event and adds it to the entity's collection of domain events.
    /// </summary>
    /// <param name="domainEvent">
    /// The domain event to trigger, representing a meaningful change or occurrence
    /// within the domain. This event will be processed by the relevant domain event handlers.
    /// </param>
    /// <remarks>
    /// This method centralizes the handling of domain events within the entity.
    /// It ensures the event is captured and made available for further processing
    /// or integration with the entity's domain event lifecycle.
    /// </remarks>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
