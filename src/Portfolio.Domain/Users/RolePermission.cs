namespace Portfolio.Domain.Users;

/// <summary>
/// Represents the link between a role and a permission in the system.
/// </summary>
/// <remarks>
/// This class establishes the connection between roles and permissions,
/// enabling effective management of access rights within the application.
/// </remarks>
public sealed class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
