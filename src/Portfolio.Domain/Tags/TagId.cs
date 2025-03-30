namespace Portfolio.Domain.Tags;

public record TagId(string Value)
{
    public static TagId New() => new($"t_{Ulid.NewUlid().ToString()}");
}
