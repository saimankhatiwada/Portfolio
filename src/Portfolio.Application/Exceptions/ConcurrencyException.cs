namespace Portfolio.Application.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a concurrency conflict occurs during data operations.
/// </summary>
public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, Exception innerException): base(message, innerException)
    {
    }
}
