namespace Portfolio.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public const string Name = "Outbox";
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
