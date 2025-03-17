using Portfolio.Application.Abstractions.Caching;
using Portfolio.Application.Users.Shared;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Application.Users.GetUsers;

/// <summary>
/// Represents a query to retrieve a list of users, optionally filtered by search criteria and sorted by a specified parameter.
/// </summary>
/// <remarks>
/// This query supports caching mechanisms through the <see cref="ICachedQuery{TResponse}"/> interface, 
/// allowing the result to be cached for improved performance. The cache key is dynamically generated 
/// based on the query parameters, and the cache expiration is set to 2 minutes.
/// </remarks>
public sealed record GetUsersQuery(string? Search, string? Sort, string? Field, int? Page, int? PageSize) : ICachedQuery<PaginationResult<UserResponse>>
{
    public string CacheKey => $"{nameof(GetUsersQuery)}-{Search}-{Sort}-{Field}-{Page}-{PageSize}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}
