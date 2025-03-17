namespace Portfolio.Domain.Users;

/// <summary>
/// Represents a strongly-typed unique identifier for a user.
/// </summary>
/// <remarks>
/// The <see cref="UserId"/> record ensures type safety by encapsulating user identifiers,
/// avoiding the risks associated with using plain strings for identification.
/// It includes functionality to create new, uniquely generated identifiers.
/// </remarks>
public record UserId(string Value)
{
    /// <summary>
    /// Generates a new <see cref="UserId"/> instance with a unique identifier.
    /// </summary>
    /// <returns>A newly created <see cref="UserId"/> containing a unique value.</returns>
    /// <remarks>
    /// This method creates a unique identifier in ULID format, prefixed with "u_".
    /// It guarantees that each <see cref="UserId"/> is unique and suitable for user identification.
    /// </remarks>
    public static UserId New() => new($"u_{Ulid.NewUlid().ToString()}");
}
