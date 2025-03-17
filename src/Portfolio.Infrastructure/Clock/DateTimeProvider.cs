using Portfolio.Application.Abstractions.Clock;

namespace Portfolio.Infrastructure.Clock;

/// <summary>
/// Provides an implementation of <see cref="IDateTimeProvider"/> to retrieve the current date and time.
/// </summary>
/// <remarks>
/// This class serves as the default implementation of <see cref="IDateTimeProvider"/>, 
/// offering access to the current UTC date and time. It is registered as a transient service 
/// in the dependency injection container to ensure consistent and testable date-time handling 
/// across the application.
/// </remarks>
internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
