using Portfolio.Application.Abstractions.Caching;
using Portfolio.Application.Model.User;

namespace Portfolio.Application.Users.GetUser;

public sealed record GetUserQuery(string Id, string? Fields) : ICachedQuery<UserDto>
{
    public string CacheKey => $"{nameof(GetUserQuery)}-{Id}-{Fields}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}
