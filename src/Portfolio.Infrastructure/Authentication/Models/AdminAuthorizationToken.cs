using System.Text.Json.Serialization;

namespace Portfolio.Infrastructure.Authentication.Models;

/// <summary>
/// Represents an authorization token for administrative access.
/// </summary>
/// <remarks>
/// This class is used to deserialize the JSON response containing the access token
/// required for administrative authorization in the application.
/// </remarks>
internal sealed class AdminAuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
}
