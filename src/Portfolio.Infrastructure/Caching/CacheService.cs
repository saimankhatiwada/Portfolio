using Microsoft.Extensions.Caching.Distributed;
using System.Buffers;
using System.Text.Json;
using Portfolio.Application.Abstractions.Caching;

namespace Portfolio.Infrastructure.Caching;

/// <summary>
/// Provides an implementation of the <see cref="ICacheService"/> interface for managing distributed caching.
/// </summary>
/// <remarks>
/// This class leverages the <see cref="IDistributedCache"/> to store, retrieve, and remove cached data.
/// It supports serialization and deserialization of cached values to and from byte arrays.
/// </remarks>
internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Retrieves a cached value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve from the cache.</typeparam>
    /// <param name="key">The key identifying the cached value.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the cached value of type <typeparamref name="T"/> 
    /// if found; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method attempts to retrieve a value from the cache. If the value is not found, it returns <c>null</c>.
    /// The value is deserialized from a byte array to the specified type <typeparamref name="T"/>.
    /// </remarks>
    /// <exception cref="System.Text.Json.JsonException">
    /// Thrown if the cached value cannot be deserialized into the specified type <typeparamref name="T"/>.
    /// </exception>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await _cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    /// <summary>
    /// Asynchronously sets a value in the distributed cache with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to be cached.</typeparam>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="expiration">
    /// An optional <see cref="TimeSpan"/> specifying the expiration time for the cached item.
    /// If not provided, the default expiration policy will be applied.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method serializes the provided value and stores it in the distributed cache.
    /// It uses the specified expiration time or a default expiration policy if none is provided.
    /// </remarks>
    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return _cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
    }

    /// <summary>
    /// Removes the cached value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the cached item to remove.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    /// <remarks>
    /// This method deletes the cached item identified by the provided key from the underlying distributed cache.
    /// </remarks>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(key, cancellationToken);

    /// <summary>
    /// Deserializes the specified byte array into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="bytes">The byte array containing the serialized object data.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="System.Text.Json.JsonException">
    /// Thrown if the byte array cannot be deserialized into an object of type <typeparamref name="T"/>.
    /// </exception>
    /// <remarks>
    /// This method uses <see cref="System.Text.Json.JsonSerializer"/> for deserialization.
    /// Ensure that the byte array represents a valid JSON structure compatible with the type <typeparamref name="T"/>.
    /// </remarks>
    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    /// <summary>
    /// Serializes the specified value into a byte array using JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>A byte array representing the serialized value.</returns>
    /// <remarks>
    /// This method utilizes <see cref="System.Text.Json.JsonSerializer"/> for efficient and compact JSON serialization.
    /// It writes the serialized data into a buffer and returns the resulting byte array.
    /// </remarks>
    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();

        using var writer = new Utf8JsonWriter(buffer);

        JsonSerializer.Serialize(writer, value);

        return buffer.WrittenSpan.ToArray();
    }
}
