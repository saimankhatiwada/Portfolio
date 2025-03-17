using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstractions.Authentication;

namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// Represents the user context implementation that provides access to user-specific information
/// within the current HTTP context.
/// </summary>
/// <remarks>
/// This class is responsible for retrieving user identifiers, such as <see cref="UserId"/> and 
/// <see cref="IdentityId"/>, from the claims principal associated with the current HTTP request.
/// </remarks>
internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the unique identifier of the current user from the claims principal associated with the current HTTP context.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the user ID.
    /// </value>
    /// <exception cref="ApplicationException">
    /// Thrown when the user context is unavailable or the user ID cannot be retrieved.
    /// </exception>
    public string UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("User context is unavailable");

    /// <summary>
    /// Gets the identity ID of the current user from the claims principal associated with the current HTTP context.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the identity ID of the current user.
    /// </value>
    /// <exception cref="ApplicationException">
    /// Thrown when the identity ID is unavailable in the current HTTP context.
    /// </exception>
    public string IdentityId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetIdentityId() ??
        throw new ApplicationException("User context is unavailable");
}
