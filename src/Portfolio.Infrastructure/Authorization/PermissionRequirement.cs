using Microsoft.AspNetCore.Authorization;

namespace Portfolio.Infrastructure.Authorization;

/// <summary>
/// Represents a requirement for a specific permission within the authorization framework.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IAuthorizationRequirement"/> interface and is used to define
/// a permission-based requirement for authorization policies. It encapsulates the permission name
/// that must be satisfied for the requirement to be fulfilled.
/// </remarks>
internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}
