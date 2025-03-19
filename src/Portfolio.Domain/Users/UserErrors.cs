using Portfolio.Domain.Abstractions;

namespace Portfolio.Domain.Users;

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
        "Keycloak server error occured while processing");
}
