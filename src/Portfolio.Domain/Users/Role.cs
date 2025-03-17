namespace Portfolio.Domain.Users;

/// <summary>
/// Represents a role within the system, defining permissions and user associations.
/// </summary>
/// <remarks>
/// The <see cref="Role"/> class includes predefined roles such as <see cref="None"/>,
/// <see cref="Registered"/>, and <see cref="SuperAdmin"/>.
/// It provides methods for role validation and retrieval, ensuring consistent role management.
/// </remarks>
public sealed class Role
{
    public static readonly Role None = new(0, string.Empty);
    public static readonly Role Registered = new(1, "Registered");
    public static readonly Role SuperAdmin = new(2, "SuperAdmin");

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }
    public ICollection<User> Users { get; init; } = [];
    public ICollection<Permission> Permissions { get; init; } = [];
    /// <summary>
    /// Retrieves a <see cref="Role"/> instance by its name.
    /// </summary>
    /// <param name="name">The name of the role to locate.</param>
    /// <returns>
    /// The <see cref="Role"/> instance corresponding to the given name.
    /// </returns>
    /// <exception cref="ApplicationException">
    /// Thrown if no predefined role matches the specified name.
    /// </exception>
    /// <remarks>
    /// This method searches the predefined roles in <see cref="Role.All"/>.
    /// If no match is found, an exception is raised to signal an invalid role.
    /// </remarks>
    public static Role FormRole(string name) => All.FirstOrDefault(r => r.Name == name) ??
                                                throw new ApplicationException("The role is invalid.");
    /// <summary>
    /// Retrieves a <see cref="Role"/> instance by its name, returning <see cref="Role.None"/> if no match is found.
    /// </summary>
    /// <param name="name">The name of the role to locate and validate.</param>
    /// <returns>
    /// The <see cref="Role"/> instance matching the specified name, or <see cref="Role.None"/> if no predefined role matches.
    /// </returns>
    /// <remarks>
    /// This method searches through the predefined roles in <see cref="Role.All"/>.
    /// If no match is found, it safely returns <see cref="Role.None"/> instead of raising an exception.
    /// </remarks>
    public static Role CheckRole(string name) => All.FirstOrDefault(r => r.Name == name) ?? None;
    public static readonly IReadOnlyCollection<Role> All =
    [
        Registered,
        SuperAdmin
    ];
}
