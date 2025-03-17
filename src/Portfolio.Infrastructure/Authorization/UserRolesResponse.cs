using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Authorization;

/// <summary>
/// Represents the response containing a user's roles within the authorization infrastructure.
/// </summary>
/// <remarks>
/// This class is used to encapsulate the user's unique identifier and their associated roles.
/// It is primarily utilized in the process of retrieving and transforming user claims.
/// </remarks>
internal sealed class UserRolesResponse
{
    public string UserId { get; init; }
    public List<Role> Roles { get; init; } = [];
}
