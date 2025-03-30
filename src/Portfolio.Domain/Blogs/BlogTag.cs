using Portfolio.Domain.Tags;

namespace Portfolio.Domain.Blogs;

public sealed class BlogTag
{
    public BlogId BlogId { get; set; }
    public TagId TagId { get; set; }
}
