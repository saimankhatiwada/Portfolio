using Microsoft.Extensions.Options;
using Quartz;

namespace Portfolio.Infrastructure.Outbox;

/// <summary>
/// Configures the Quartz job and trigger for processing outbox messages.
/// </summary>
/// <remarks>
/// The <see cref="ProcessOutboxMessagesJobSetup"/> class is responsible for setting up the Quartz job
/// <see cref="ProcessOutboxMessagesJob"/> and its associated trigger. It uses the configuration
/// provided by <see cref="OutboxOptions"/> to determine the job's execution interval.
/// This setup ensures that outbox messages are processed at regular intervals as defined
/// in the application's configuration.
/// </remarks>
internal sealed class ProcessOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _outboxOptions;

    public ProcessOutboxMessagesJobSetup(IOptions<OutboxOptions> outboxOptions)
    {
        _outboxOptions = outboxOptions.Value;
    }

    /// <summary>
    /// Configures the Quartz options to set up the job and trigger for processing outbox messages.
    /// </summary>
    /// <param name="options">The <see cref="QuartzOptions"/> instance to be configured.</param>
    /// <remarks>
    /// This method adds the <see cref="ProcessOutboxMessagesJob"/> to the Quartz scheduler
    /// and configures its trigger to execute at intervals defined by the <see cref="OutboxOptions.IntervalInSeconds"/>.
    /// The job is identified by its name and is set to repeat indefinitely.
    /// </remarks>
    public void Configure(QuartzOptions options)
    {
        const string jobName = nameof(ProcessOutboxMessagesJob);

        options
            .AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));
    }
}
