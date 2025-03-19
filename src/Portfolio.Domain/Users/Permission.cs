namespace Portfolio.Domain.Users;

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
