namespace Portfolio.Infrastructure.Outbox;

/// <summary>
/// Represents a message in the outbox pattern, used to ensure reliable message delivery in distributed systems.
/// </summary>
/// <remarks>
/// This class encapsulates the details of an outbox message, including its unique identifier, 
/// the time it occurred, its type, content, and optional processing metadata such as the time it was processed 
/// and any associated error information.
/// </remarks>
public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Content = content;
        Type = type;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }

    public string Type { get; init; }

    public string Content { get; init; }

    public DateTime? ProcessedOnUtc { get; init; }

    public string? Error { get; init; }
}
