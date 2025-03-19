using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Authorization;

internal sealed class UserRolesResponse
{
    public string UserId { get; init; }
    public List<Role> Roles { get; init; } = [];
}
