namespace Portfolio.Domain.Tags;

public sealed record Name(string Value)
{
    public static implicit operator string(Name name) => name.Value;
}
