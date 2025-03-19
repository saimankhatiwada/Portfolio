using Portfolio.Domain.Abstractions;

namespace Portfolio.Domain.Users.Events;

public sealed record UserRegisteredDomainEvent(UserId UserId) : IDomainEvent;
