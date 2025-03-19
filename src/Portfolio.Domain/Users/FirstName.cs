namespace Portfolio.Domain.Users;

public sealed record FirstName(string Value)
{
    public static implicit operator string(FirstName firstName) => firstName.Value;
}
