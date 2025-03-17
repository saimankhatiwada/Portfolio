using MediatR;

namespace Portfolio.Domain.Abstractions;

/// <summary>
/// Defines a domain event within the domain layer.
/// </summary>
/// <remarks>
/// A domain event represents a significant change or occurrence in the domain's state.
/// This interface serves as a marker for such events and leverages the MediatR library
/// to enable event-driven workflows and processing.
/// </remarks>
public interface IDomainEvent : INotification
{
}
