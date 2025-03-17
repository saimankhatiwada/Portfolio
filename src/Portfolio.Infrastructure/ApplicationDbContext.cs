using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Portfolio.Application.Abstractions.Clock;
using Portfolio.Application.Exceptions;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;
using Portfolio.Infrastructure.Outbox;

namespace Portfolio.Infrastructure;

/// <summary>
/// Represents the database context for the Portfolio application, providing access to the database
/// and managing entity configurations and transactions.
/// </summary>
/// <remarks>
/// This class extends <see cref="DbContext"/> and implements <see cref="Portfolio.Domain.Abstractions.IUnitOfWork"/>.
/// It is responsible for configuring the database schema, applying entity configurations, and managing
/// database transactions.
/// </remarks>
/// <example>
/// To use this class, inject it into your services or repositories:
/// <code>
/// public class UserService
/// {
///     private readonly ApplicationDbContext _dbContext;
///     
///     public UserService(ApplicationDbContext dbContext)
///     {
///         _dbContext = dbContext;
///     }
/// }
/// </code>
/// </example>
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IDateTimeProvider _dateTimeProvider;
    
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider) : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Configures the model for the database context during the model creation phase.
    /// </summary>
    /// <param name="modelBuilder">
    /// An instance of <see cref="ModelBuilder"/> used to define the shape of entities, relationships, and database schema.
    /// </param>
    /// <remarks>
    /// This method applies configurations from the assembly containing the <see cref="ApplicationDbContext"/> class
    /// and sets the default schema for the database to "portfolio".
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the save operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains the number of state entries 
    /// written to the database.
    /// </returns>
    /// <exception cref="ConcurrencyException">
    /// Thrown when a concurrency conflict is detected during the save operation.
    /// </exception>
    /// <remarks>
    /// This method overrides <see cref="DbContext.SaveChangesAsync"/> to include additional functionality, such as 
    /// processing domain events as outbox messages before saving changes.
    /// </remarks>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();

            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    /// <summary>
    /// Converts domain events from tracked entities into outbox messages for reliable processing.
    /// </summary>
    /// <remarks>
    /// This method iterates through all tracked entities implementing <see cref="IEntity"/>, 
    /// retrieves their domain events, and converts these events into instances of <see cref="OutboxMessage"/>.
    /// The domain events are then cleared from the entities to ensure they are not processed multiple times.
    /// </remarks>
    /// <exception cref="JsonSerializationException">
    /// Thrown if there is an error during the serialization of a domain event.
    /// </exception>
    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<IEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyList<IDomainEvent> domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}
