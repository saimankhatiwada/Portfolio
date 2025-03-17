namespace Portfolio.Application.Abstractions.Clock;

/// <summary>
/// Provides an abstraction for accessing the current date and time.
/// </summary>
/// <remarks>
/// This interface is intended to provide a centralized way to retrieve the current date and time,
/// ensuring consistency and enabling easier testing by allowing the use of mock implementations.
/// </remarks>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
