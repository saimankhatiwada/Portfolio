using Portfolio.Domain.Abstractions;

namespace Portfolio.Domain.Users;

/// <summary>
/// Defines a set of standardized errors associated with user operations.
/// </summary>
/// <remarks>
/// This class provides static instances of the <see cref="Error"/> type, representing common user-related issues.
/// Examples include user not found, invalid credentials, email conflicts, and server-related errors.
/// </remarks>
public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound",
        "The user with specified identifier was not found");

    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid");

    public static readonly Error EmailConflict = new(
        "User.EmailConflict",
        "The user with provided email already exists");

    public static readonly Error RefreshToken = new(
        "User.RefreshToken",
        "The user provided invalid refresh token");

    public static readonly Error KeycloakServerError = new(
        "User.KeycloakServerError",
        "Keycloak server error occured while creating user");
}
