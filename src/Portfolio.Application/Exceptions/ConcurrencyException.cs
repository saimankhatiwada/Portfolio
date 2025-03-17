namespace Portfolio.Application.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a concurrency conflict occurs during data operations.
/// </summary>
/// <remarks>
/// This exception is typically used to handle scenarios where multiple processes or users attempt to modify the same data simultaneously,
/// resulting in a conflict. It wraps the underlying <see cref="DbUpdateConcurrencyException"/> to provide a more domain-specific context.
/// </remarks>
public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, Exception innerException): base(message, innerException)
    {
    }
}
