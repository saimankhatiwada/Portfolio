using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// Provides extension methods for the <see cref="ClaimsPrincipal"/> class to retrieve user-related claims.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the user ID from the claims of the specified <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance from which to retrieve the user ID.</param>
    /// <returns>The user ID as a <see cref="string"/>.</returns>
    /// <exception cref="ApplicationException">Thrown when the user ID is unavailable.</exception>
    public static string GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return userId ?? throw new ApplicationException("User id is unavailable");
    }

    /// <summary>
    /// Retrieves the identity ID from the claims of the specified <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance from which to retrieve the identity ID.</param>
    /// <returns>The identity ID as a <see cref="string"/>.</returns>
    /// <exception cref="ApplicationException">Thrown when the identity ID is unavailable.</exception>
    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
               throw new ApplicationException("User identity is unavailable");
    }
}
