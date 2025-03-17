namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// Represents the configuration options for integrating with a Keycloak authentication server.
/// </summary>
/// <remarks>
/// This class provides properties to configure Keycloak-specific settings such as URLs, client IDs, and secrets.
/// It is typically bound to a configuration section named <see cref="Name"/> in the application's configuration file.
/// </remarks>
public sealed class KeycloakOptions
{
    public const string Name = "Keycloak";

    public string AdminUrl { get; set; } = string.Empty;

    public string TokenUrl { get; set; } = string.Empty;

    public string AdminClientId { get; init; } = string.Empty;

    public string AdminClientSecret { get; init; } = string.Empty;

    public string AuthClientId { get; init; } = string.Empty;

    public string AuthClientSecret { get; init; } = string.Empty;
}
