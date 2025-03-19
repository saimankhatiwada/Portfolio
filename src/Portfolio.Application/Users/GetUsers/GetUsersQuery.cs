using Portfolio.Application.Abstractions.Caching;
using Portfolio.Application.Model.User;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Application.Users.GetUsers;

public sealed record GetUsersQuery(string? Search, string? Sort, string? Fields, int? Page, int? PageSize) : ICachedQuery<PaginationResult<UserDto>>
{
    public string CacheKey => $"{nameof(GetUsersQuery)}-{Search}-{Sort}-{Fields}-{Page}-{PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}
