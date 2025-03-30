using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Tags;
using Portfolio.Domain.Users;

namespace Portfolio.Domain.Blogs;

public sealed class Blog : Entity<BlogId>
{
    private readonly List<BlogTag> _blogTags = [];
    private readonly List<Tag> _tags = [];

    private Blog(BlogId id, UserId userId, Tittle tittle, Content content, Summary summary, Status status, 
        DateTime publishedAtUtc) : base(id)
    {
        UserId = userId;
        Tittle = tittle;
        Content = content;
        Summary = summary;
        Status = status;
        PublishedAtUtc = publishedAtUtc;
    }

    private Blog() {}
    public UserId UserId { get; private set; }
    public Tittle Tittle { get; private set; }
    public Content Content { get; private set; }
    public Summary Summary { get; private set; }
    public Status Status { get; private set; }
    public DateTime PublishedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public IReadOnlyCollection<BlogTag> BlogTags => [.._blogTags];
    public IReadOnlyCollection<Tag> Tags => [.._tags];


    public static Blog AddBlog(UserId userId, Tittle tittle, Content content, Summary summary, Status status,
        DateTime publishedAtUtc)
    {
        var blog = new Blog(BlogId.New(), userId, tittle, content, summary, status, publishedAtUtc);

        return blog;
    }
}
