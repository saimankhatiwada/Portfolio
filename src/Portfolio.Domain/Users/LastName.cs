namespace Portfolio.Domain.Users;

/// <summary>
/// Represents the last name of a user within the domain.
/// </summary>
/// <remarks>
/// The <see cref="LastName"/> record defines a user's last name as
/// a immutable value object, preserving its consistency in the domain.
/// </remarks>
public sealed record LastName(string Value)
{
    public static implicit operator string(LastName lastName) => lastName.Value;
}
