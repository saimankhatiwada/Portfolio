namespace Portfolio.Domain.Users;

public record UserId(string Value)
{
    public static UserId New() => new($"u_{Ulid.NewUlid().ToString()}");
}
