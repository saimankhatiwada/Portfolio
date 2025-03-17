using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Authentication;

namespace Portfolio.Infrastructure.Authorization;

/// <summary>
/// Handles authorization requirements based on user permissions.
/// </summary>
/// <remarks>
/// This class is responsible for evaluating <see cref="PermissionRequirement"/> instances
/// to determine whether a user has the necessary permissions to access a resource.
/// It utilizes the <see cref="AuthorizationService"/> to fetch the user's permissions
/// and validates them against the required permission.
/// </remarks>
internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Handles the authorization requirement for a specific permission.
    /// </summary>
    /// <param name="context">
    /// The <see cref="AuthorizationHandlerContext"/> containing information about the authorization process,
    /// including the user and the resource being accessed.
    /// </param>
    /// <param name="requirement">
    /// The <see cref="PermissionRequirement"/> that specifies the required permission for the authorization.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation of handling the requirement.
    /// </returns>
    /// <remarks>
    /// This method checks if the user is authenticated and retrieves the user's permissions using the
    /// <see cref="AuthorizationService"/>. If the user possesses the required permission, the requirement
    /// is marked as succeeded.
    /// </remarks>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            return;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();

        AuthorizationService authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        string identityId = context.User.GetIdentityId();

        HashSet<string> permissions = await authorizationService.GetPermissionsForUserAsync(identityId);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
