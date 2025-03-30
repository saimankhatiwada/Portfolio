namespace Portfolio.Domain.Users;

public sealed class Permission
{
    public static readonly Permission UsersReadSelf = new(1, "users:read-self");
    public static readonly Permission UsersRead = new(2, "users:read");
    public static readonly Permission UsersReadSingle = new(3, "users:read-single");
    public static readonly Permission UsersUpdate = new(4, "users:update");
    public static readonly Permission UsersDelete = new(5, "users:delete");
    public static readonly Permission TagsRead = new(6, "tags:read");
    public static readonly Permission TagsReadSingle = new(7, "tags:read-single");
    public static readonly Permission TagsAdd = new(8, "tags:add");
    public static readonly Permission TagsUpdate = new(9, "tags:update");
    public static readonly Permission TagsDelete = new(10, "tags:delete");
    public static readonly Permission BlogsRead = new(11, "blogs:read");
    public static readonly Permission BlogsReadSingle = new(12, "blogs:read-single");
    public static readonly Permission BlogsAdd = new(13, "blogs:add");
    public static readonly Permission BlogsUpdate = new(14, "blogs:update");
    public static readonly Permission BlogsDelete = new(15, "blogs:delete");
    
    private Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public int Id { get; init; }
    public string Name { get; init; }
}
