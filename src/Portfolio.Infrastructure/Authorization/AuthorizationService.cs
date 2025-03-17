using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Abstractions.Caching;
using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Authorization;

/// <summary>
/// Provides authorization-related services, including retrieving user roles and permissions.
/// </summary>
/// <remarks>
/// This service interacts with the database context and caching mechanisms to efficiently
/// fetch and cache user roles and permissions. It is primarily used to support authorization
/// operations within the application.
/// </remarks>
internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public AuthorizationService(ApplicationDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Retrieves the roles associated with a user based on their identity ID.
    /// </summary>
    /// <param name="identityId">The unique identity ID of the user.</param>
    /// <returns>
    /// A <see cref="UserRolesResponse"/> object containing the user's ID and their associated roles.
    /// </returns>
    /// <remarks>
    /// This method first attempts to retrieve the roles from the cache. If the roles are not found in the cache,
    /// it queries the database to fetch the roles, caches the result, and then returns it.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user with the specified identity ID does not exist in the database.
    /// </exception>
    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        string cacheKey = $"auth:roles-{identityId}";
        UserRolesResponse? cachedRoles = await _cacheService.GetAsync<UserRolesResponse>(cacheKey);

        if (cachedRoles is not null)
        {
            return cachedRoles;
        }

        UserRolesResponse roles = await _dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .Select(u => new UserRolesResponse
            {
                UserId = u.Id.Value,
                Roles = u.Roles.ToList()
            })
            .FirstAsync();

        await _cacheService.SetAsync(cacheKey, roles);

        return roles;
    }

    /// <summary>
    /// Retrieves the set of permissions associated with a user based on their identity.
    /// </summary>
    /// <param name="identityId">The unique identifier of the user whose permissions are to be retrieved.</param>
    /// <returns>
    /// A <see cref="HashSet{T}"/> containing the names of the permissions assigned to the user.
    /// </returns>
    /// <remarks>
    /// This method first attempts to retrieve the permissions from the cache. If the permissions
    /// are not found in the cache, it queries the database for the user's roles and their associated
    /// permissions, caches the result, and then returns the permissions.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user with the specified <paramref name="identityId"/> does not exist in the database.
    /// </exception>
    /// <example>
    /// <code>
    /// var permissions = await authorizationService.GetPermissionsForUserAsync("user-123");
    /// if (permissions.Contains("AdminAccess"))
    /// {
    ///     // Grant admin access
    /// }
    /// </code>
    /// </example>
    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        string cacheKey = $"auth:permissions-{identityId}";
        HashSet<string>? cachedPermissions = await _cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachedPermissions is not null)
        {
            return cachedPermissions;
        }

        ICollection<Permission> permissions = await _dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .SelectMany(u => u.Roles.Select(r => r.Permissions))
            .FirstAsync();

        var permissionsSet = permissions.Select(p => p.Name).ToHashSet();

        await _cacheService.SetAsync(cacheKey, permissionsSet);

        return permissionsSet;
    }
}
