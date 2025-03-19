using System.Text.Json.Serialization;

namespace Portfolio.Application.Abstractions.Authentication;

public sealed class AuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }
}
