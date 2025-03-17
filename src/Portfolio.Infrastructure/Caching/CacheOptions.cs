using Microsoft.Extensions.Caching.Distributed;

namespace Portfolio.Infrastructure.Caching;

/// <summary>
/// Provides utility methods for creating and managing cache entry options.
/// </summary>
/// <remarks>
/// This static class simplifies the creation of <see cref="DistributedCacheEntryOptions"/> 
/// with predefined or custom expiration settings. It is designed to streamline cache configuration 
/// and ensure consistent behavior across the application.
/// </remarks>
public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    /// <summary>
    /// Creates a new instance of <see cref="DistributedCacheEntryOptions"/> with the specified expiration settings.
    /// </summary>
    /// <param name="expiration">
    /// The relative expiration time for the cache entry. If <c>null</c>, the default expiration 
    /// (<see cref="DefaultExpiration"/>) will be used.
    /// </param>
    /// <returns>
    /// A <see cref="DistributedCacheEntryOptions"/> instance configured with the provided expiration 
    /// or the default expiration if none is specified.
    /// </returns>
    /// <remarks>
    /// This method simplifies the creation of cache entry options by allowing the caller to specify 
    /// a custom expiration or rely on a predefined default. It ensures consistent behavior when 
    /// configuring cache entries.
    /// </remarks>
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null ?
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } :
            DefaultExpiration;
}
