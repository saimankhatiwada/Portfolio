using System.Text.Json.Serialization;

namespace Portfolio.Infrastructure.Authentication.Models;

internal sealed class AdminAuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
}
