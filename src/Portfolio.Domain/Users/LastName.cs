namespace Portfolio.Domain.Users;

public sealed record LastName(string Value)
{
    public static implicit operator string(LastName lastName) => lastName.Value;
}
