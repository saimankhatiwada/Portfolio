using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Portfolio.Application.Abstractions.Clock;
using Portfolio.Application.Abstractions.Data;
using Portfolio.Domain.Abstractions;
using System.Data;
using Quartz;
using Dapper;

namespace Portfolio.Infrastructure.Outbox;

/// <summary>
/// Represents a Quartz job responsible for processing outbox messages.
/// </summary>
/// <remarks>
/// This class implements the <see cref="Quartz.IJob"/> interface and is designed to handle
/// the processing of messages stored in the outbox table. It retrieves messages, publishes
/// them as domain events, and updates their status in the database.
/// </remarks>
internal sealed class ProcessOutboxMessagesJob : IJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IPublisher _publisher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _outboxOptions = outboxOptions.Value;
    }

    /// <summary>
    /// Executes the Quartz job to process outbox messages.
    /// </summary>
    /// <param name="context">
    /// The execution context provided by Quartz, containing details about the job execution.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of processing outbox messages.
    /// </returns>
    /// <remarks>
    /// This method retrieves outbox messages from the database, deserializes their content into domain events,
    /// publishes the events, and updates the status of the messages in the outbox table. It ensures that
    /// all operations are performed within a database transaction for consistency.
    /// </remarks>
    /// <exception cref="System.Exception">
    /// Thrown if an error occurs during the processing of an outbox message.
    /// </exception>
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages");

        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        using IDbTransaction transaction = connection.BeginTransaction();

        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    JsonSerializerSettings)!;

                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "Exception while processing outbox message {MessageId}",
                    outboxMessage.Id);

                exception = caughtException;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");
    }

    /// <summary>
    /// Retrieves a batch of unprocessed outbox messages from the database.
    /// </summary>
    /// <param name="connection">The database connection to use for the query.</param>
    /// <param name="transaction">The database transaction to use for the query.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list
    /// of <see cref="OutboxMessageResponse"/> objects representing the unprocessed outbox messages.
    /// </returns>
    /// <remarks>
    /// This method queries the outbox table for messages that have not been processed yet, 
    /// ordered by their occurrence time. It uses a batch size defined in <see cref="OutboxOptions.BatchSize"/> 
    /// and applies row-level locking to ensure messages are not processed concurrently by multiple workers.
    /// </remarks>
    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql = $"""
                      SELECT id, content
                      FROM outbox_messages
                      WHERE processed_on_utc IS NULL
                      ORDER BY occurred_on_utc
                      LIMIT {_outboxOptions.BatchSize}
                      FOR UPDATE SKIP LOCKED
                      """;

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            transaction: transaction);

        return outboxMessages.ToList();
    }

    /// <summary>
    /// Updates the status of an outbox message in the database after processing.
    /// </summary>
    /// <param name="connection">The database connection used to execute the update query.</param>
    /// <param name="transaction">The database transaction within which the update query is executed.</param>
    /// <param name="outboxMessage">The outbox message being updated, containing its identifier and content.</param>
    /// <param name="exception">
    /// An optional exception that occurred during the processing of the outbox message. 
    /// If provided, its details will be stored in the database.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method updates the `processed_on_utc` timestamp and optionally logs any error information 
    /// for the specified outbox message in the database.
    /// </remarks>
    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = """
                           UPDATE outbox_messages
                           SET processed_on_utc = @ProcessedOnUtc,
                               error = @Error
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    /// <summary>
    /// Represents a response containing details of an outbox message.
    /// </summary>
    /// <remarks>
    /// This record is used to encapsulate the data of an outbox message, including its unique identifier
    /// and content, which is typically serialized domain event data.
    /// </remarks>
    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
