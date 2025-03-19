namespace Portfolio.Domain.Users;

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

    public static Role FormRole(string name) => All.FirstOrDefault(r => r.Name == name) ??
                                                throw new ApplicationException("The role is invalid.");
    public static Role CheckRole(string name) => All.FirstOrDefault(r => r.Name == name) ?? None;
    public static readonly IReadOnlyCollection<Role> All =
    [
        Registered,
        SuperAdmin
    ];
}
