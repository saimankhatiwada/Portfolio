using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Portfolio.Infrastructure.Authorization;

/// <summary>
/// Provides a custom implementation of <see cref="DefaultAuthorizationPolicyProvider"/> 
/// to dynamically create and manage authorization policies based on permissions.
/// </summary>
/// <remarks>
/// This class extends the default policy provider to support permission-based authorization.
/// It dynamically generates policies for permissions that are not pre-defined in the 
/// <see cref="AuthorizationOptions"/> and adds them to the options for future use.
/// </remarks>
internal sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptions;

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _authorizationOptions = options.Value;
    }

    /// <summary>
    /// Retrieves the <see cref="AuthorizationPolicy"/> for the specified policy name.
    /// </summary>
    /// <param name="policyName">The name of the policy to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the 
    /// <see cref="AuthorizationPolicy"/> associated with the specified policy name, or a dynamically 
    /// created policy if none exists.
    /// </returns>
    /// <remarks>
    /// If the policy with the specified name is not found in the existing policies, this method 
    /// dynamically creates a new policy using a <see cref="PermissionRequirement"/> for the given 
    /// policy name. The newly created policy is then added to the <see cref="AuthorizationOptions"/> 
    /// for future use.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="policyName"/> is <c>null</c>.
    /// </exception>
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }

        AuthorizationPolicy permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        _authorizationOptions.AddPolicy(policyName, permissionPolicy);

        return permissionPolicy;
    }
}
