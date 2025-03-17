using Microsoft.AspNetCore.Authorization;

namespace Portfolio.Infrastructure.Authorization;

/// <summary>
/// Represents an attribute that specifies a required permission for accessing a method.
/// </summary>
/// <remarks>
/// This attribute is used to enforce authorization policies by specifying the required permission
/// for a method. It inherits from <see cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(permission)
    {
    }
}
