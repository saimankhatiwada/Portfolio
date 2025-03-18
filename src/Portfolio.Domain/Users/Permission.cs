namespace Portfolio.Domain.Users;

/// <summary>
/// Represents a domain-specific permission, defining actions or access rights
/// that can be granted to roles or users.
/// </summary>
/// <remarks>
/// The <see cref="Permission"/> class models a permission with a unique identifier
/// and a descriptive name. It includes predefined static instances for common actions,
/// such as reading, updating, or deleting user data. This class is immutable,
/// ensuring permissions are consistently defined and uniquely identifiable.
/// </remarks>
public sealed class Permission
{
    public static readonly Permission UsersReadSelf = new(1, "users:read-self");
    public static readonly Permission UsersRead = new(2, "users:read");
    public static readonly Permission UsersReadSingle = new(3, "users:read-single");
    public static readonly Permission UsersUpdate = new(4, "users:update");
    public static readonly Permission UsersDelete = new(5, "users:delete");
    
    private Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }
}
