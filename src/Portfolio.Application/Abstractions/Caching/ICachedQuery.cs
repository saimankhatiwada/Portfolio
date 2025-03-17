using Portfolio.Application.Abstractions.Messaging;

namespace Portfolio.Application.Abstractions.Caching;

/// <summary>
/// Represents a query that supports caching mechanisms, allowing the result of the query 
/// to be stored and retrieved from a cache for improved performance and efficiency.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response produced by the query.
/// </typeparam>
/// <remarks>
/// This interface extends the <see cref="IQuery{TResponse}"/> interface to include caching capabilities. 
/// It provides a mechanism to define a cache key and optional expiration for the cached query result.
/// </remarks>
public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

/// <summary>
/// Represents a query that supports caching functionality.
/// </summary>
/// <remarks>
/// Implementations of this interface define a cache key and an optional expiration time for the cached data.
/// Queries implementing this interface can be used in conjunction with caching behaviors to store and retrieve
/// results efficiently, reducing redundant processing and improving performance.
/// </remarks>
public interface ICachedQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}
