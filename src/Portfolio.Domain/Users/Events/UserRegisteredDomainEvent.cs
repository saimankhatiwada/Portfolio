using Portfolio.Domain.Abstractions;

namespace Portfolio.Domain.Users.Events;

/// <summary>
/// Represents a domain event raised after a user is successfully registered.
/// </summary>
/// <remarks>
/// The <see cref="UserRegisteredDomainEvent"/> indicates the completion of the
/// user registration process. It encapsulates the unique identifier of the
/// newly registered user and acts as a notification within the domain layer
/// to broadcast this critical event.
/// </remarks>
public sealed record UserRegisteredDomainEvent(UserId UserId) : IDomainEvent;
