using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Repositories;

/// <summary>
/// Represents a generic repository for managing entities within the application's data context.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity managed by the repository. Must inherit from <see cref="Entity{TEntityId}"/>.
/// </typeparam>
/// <typeparam name="TEntityId">
/// The type of the unique identifier for the entity. Must be a reference type.
/// </typeparam>
/// <remarks>
/// This abstract class provides a base implementation for common repository operations, such as
/// retrieving, adding, updating, and deleting entities. It leverages the application's database
/// context to perform these operations.
/// </remarks>
internal abstract class Repository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : class
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the entity
    /// if found; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method queries the database context to locate an entity of type <typeparamref name="TEntity"/>
    /// with the specified identifier. If no matching entity is found, the method returns <c>null</c>.
    /// </remarks>
    public async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    /// <summary>
    /// Adds the specified entity to the database context for tracking and persistence.
    /// </summary>
    /// <param name="entity">
    /// The entity of type <typeparamref name="TEntity"/> to be added to the database context.
    /// </param>
    /// <remarks>
    /// This method marks the provided entity as added within the database context, 
    /// so it will be included in the next save operation. The entity must conform to the 
    /// constraints defined by <typeparamref name="TEntity"/>.
    /// </remarks>
    public virtual void Add(TEntity entity)
    {
        DbContext.Add(entity);
    }

    /// <summary>
    /// Updates the specified entity in the database context.
    /// </summary>
    /// <param name="entity">
    /// The entity to update. Must be of type <typeparamref name="TEntity"/>.
    /// </param>
    /// <remarks>
    /// This method marks the provided entity as modified in the database context, 
    /// ensuring that changes to the entity are tracked and persisted to the database 
    /// during the next save operation.
    /// </remarks>
    public void Update(TEntity entity)
    {
        DbContext.Update(entity);
    }
    
    /// <summary>
    /// Removes the specified entity from the database context.
    /// </summary>
    /// <param name="entity">
    /// The entity to be removed. Must be an instance of <typeparamref name="TEntity"/>.
    /// </param>
    /// <remarks>
    /// This method marks the provided entity for deletion within the database context. 
    /// The changes will be persisted to the database when the unit of work is committed.
    /// </remarks>
    public void Delete(TEntity entity)
    {
        DbContext.Remove(entity);
    }
}
