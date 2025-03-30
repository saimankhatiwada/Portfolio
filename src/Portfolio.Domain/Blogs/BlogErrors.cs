using Portfolio.Domain.Abstractions;

namespace Portfolio.Domain.Blogs;

public static class BlogErrors
{
    public static readonly Error NotFound = new(
        "Blog.NotFound",
        "The blog with specified identifier was not found");
}
