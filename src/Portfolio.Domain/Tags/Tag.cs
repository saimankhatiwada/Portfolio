using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Domain.Tags;

public sealed class Tag : Entity<TagId>
{
    private Tag(TagId id, UserId userId, Name name, Description? description) : base(id)
    {
        UserId = userId;
        Name = name;
        Description = description;
    }

    private Tag() {}
    public UserId UserId { get; private set; }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }

    public static Tag AddTag(UserId userId, Name name, Description? description)
    {
        var tag = new Tag(TagId.New(), userId, name, description);

        return tag;
    }

    public void UpdateTag(Name name, Description? description)
    {
        Name = name;
        Description = description;
    }
}
