using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Domain.Users;
using Portfolio.Infrastructure.Authentication;

namespace Portfolio.Infrastructure.Authorization;

/// <summary>
/// Provides a custom implementation of <see cref="IClaimsTransformation"/> to enhance the claims of a 
/// <see cref="ClaimsPrincipal"/> by adding role and user-specific claims.
/// </summary>
/// <remarks>
/// This class is responsible for transforming the claims of an authenticated user by retrieving their roles 
/// and other related information from the <see cref="AuthorizationService"/>. It ensures that the claims 
/// include the user's roles and a subject identifier (<see cref="JwtRegisteredClaimNames.Sub"/>).
/// </remarks>
internal sealed class CustomClaimsTransformation : IClaimsTransformation
{
    private readonly IServiceProvider _serviceProvider;

    public CustomClaimsTransformation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Transforms the claims of the specified <see cref="ClaimsPrincipal"/> by adding user-specific claims, 
    /// such as roles and a subject identifier (<see cref="System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub"/>).
    /// </summary>
    /// <param name="principal">
    /// The <see cref="ClaimsPrincipal"/> representing the authenticated user whose claims need to be transformed.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains the 
    /// transformed <see cref="ClaimsPrincipal"/> with additional claims.
    /// </returns>
    /// <remarks>
    /// This method retrieves the user's roles and identity information using the <see cref="AuthorizationService"/> 
    /// and adds them as claims to the provided <see cref="ClaimsPrincipal"/>. If the principal already contains 
    /// the required claims, it is returned as-is.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if the required services cannot be resolved from the <see cref="IServiceProvider"/>.
    /// </exception>
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not { IsAuthenticated: true } ||
            principal.HasClaim(claim => claim.Type == ClaimTypes.Role) &&
            principal.HasClaim(claim => claim.Type == JwtRegisteredClaimNames.Sub))
        {
            return principal;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();

        AuthorizationService authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        string identityId = principal.GetIdentityId();

        UserRolesResponse userRoles = await authorizationService.GetRolesForUserAsync(identityId);

        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userRoles.UserId.ToString()));

        foreach (Role role in userRoles.Roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
        }

        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}
