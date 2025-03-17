namespace Portfolio.Application.Abstractions.Authentication;

/// <summary>
/// Provides an abstraction for accessing user-specific context information, such as user identifiers.
/// </summary>
public interface IUserContext
{
    string UserId { get; }
    string IdentityId { get; }
}
