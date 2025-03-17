namespace Portfolio.Application.Abstractions.Caching;

/// <summary>
/// Represents a caching service that provides methods for storing, retrieving, and removing data in a cache.
/// </summary>
/// <remarks>
/// This interface is designed to abstract caching operations, allowing for flexible implementations
/// and integration with various caching providers. It supports asynchronous operations for better performance
/// and scalability in modern applications.
/// </remarks>
public interface ICacheService
{
    /// <summary>
    /// Asynchronously retrieves a cached value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve from the cache.</typeparam>
    /// <param name="key">The unique key identifying the cached value.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the cached value of type <typeparamref name="T"/> 
    /// if the key exists in the cache; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method is designed to provide a non-blocking way to retrieve data from the cache. If the key does not exist in the cache,
    /// the method returns <c>null</c>.
    /// </remarks>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    /// <summary>
    /// Asynchronously stores a value in the cache with the specified key and optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to store in the cache.</typeparam>
    /// <param name="key">The unique key to associate with the cached value.</param>
    /// <param name="value">The value to store in the cache.</param>
    /// <param name="expiration">
    /// An optional <see cref="TimeSpan"/> specifying the duration the value should remain in the cache.
    /// If <c>null</c>, the default expiration policy of the cache provider will be applied.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method allows storing data in the cache for improved performance and reduced database or API calls.
    /// Use the <paramref name="expiration"/> parameter to control the lifetime of the cached value.
    /// </remarks>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Asynchronously removes a cached value associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the cached value to remove.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is used to delete a specific entry from the cache, ensuring that the associated key and value
    /// are no longer available. Use this method to manage cache invalidation and maintain data consistency.
    /// </remarks>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
