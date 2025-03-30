namespace Portfolio.Domain.Blogs;

public record BlogId(string Value)
{
    public static BlogId New() => new($"b_{Ulid.NewUlid().ToString()}");
}
