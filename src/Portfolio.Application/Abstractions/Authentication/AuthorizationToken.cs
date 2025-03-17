using System.Text.Json.Serialization;

namespace Portfolio.Application.Abstractions.Authentication;

/// <summary>
/// Represents an authorization token containing both an access token and a refresh token.
/// </summary>
/// <remarks>
/// This class is used to encapsulate the tokens required for authentication and authorization.
/// The <see cref="AccessToken"/> is used for accessing secured resources, while the <see cref="RefreshToken"/>
/// is used to obtain a new access token when the current one expires.
/// </remarks>
public sealed class AuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }
}
