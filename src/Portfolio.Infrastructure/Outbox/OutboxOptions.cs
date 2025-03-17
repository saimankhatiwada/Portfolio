namespace Portfolio.Infrastructure.Outbox;

/// <summary>
/// Represents configuration options for the outbox mechanism.
/// </summary>
/// <remarks>
/// The <see cref="OutboxOptions"/> class is used to configure the behavior of the outbox mechanism,
/// including settings such as the interval for processing messages and the batch size.
/// This configuration is typically bound to a configuration section named <see cref="Name"/>.
/// </remarks>
public sealed class OutboxOptions
{
    public const string Name = "Outbox";

    public int IntervalInSeconds { get; init; }

    public int BatchSize { get; init; }
}
