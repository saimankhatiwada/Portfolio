namespace Portfolio.Domain.Users;

/// <summary>
/// Represents a user's first name within the domain.
/// </summary>
/// <remarks>
/// The <see cref="FirstName"/> record encapsulates a user's first name as a
/// distinct and immutable value object, ensuring its integrity within the domain.
/// </remarks>
public sealed record FirstName(string Value)
{
    public static implicit operator string(FirstName firstName) => firstName.Value;
}
