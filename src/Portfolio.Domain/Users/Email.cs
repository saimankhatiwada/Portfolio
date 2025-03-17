namespace Portfolio.Domain.Users;

/// <summary>
/// Represents a user's email address within the domain.
/// </summary>
/// <remarks>
/// The <see cref="Email"/> record encapsulates the email address value, ensuring
/// it is validated, immutable, and treated as a core domain concept.
/// </remarks>
public sealed record Email(string Value);
