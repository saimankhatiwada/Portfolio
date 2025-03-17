using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// Provides functionality for handling JSON Web Token (JWT) operations, 
/// including obtaining authorization tokens from a Keycloak server for authentication purposes.
/// </summary>
/// <remarks>
/// This class interacts with the Keycloak server using HTTP requests to retrieve authorization tokens.
/// It is configured to use specific client credentials and endpoints defined in the <see cref="KeycloakOptions"/>.
/// </remarks>
internal sealed class JwtService : IJwtService
{

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;

    public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    /// <summary>
    /// Asynchronously retrieves an <see cref="AuthorizationToken"/> for the specified user credentials.
    /// </summary>
    /// <param name="email">The email address of the user attempting to authenticate.</param>
    /// <param name="password">The password of the user attempting to authenticate.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the <see cref="AuthorizationToken"/> if authentication is successful,
    /// or a failure result if authentication fails.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown if there is an issue with the HTTP request during the authentication process.
    /// </exception>
    public async Task<Result<AuthorizationToken>> GetAuthorizationTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", _keycloakOptions.AuthClientId),
                new("client_secret", _keycloakOptions.AuthClientSecret),
                new("scope", "openid email"),
                new("grant_type", "password"),
                new("username", email),
                new("password", password)
            };

            using var authorizationRequestContent = new FormUrlEncodedContent(authRequestParameters);

            HttpResponseMessage response = await _httpClient.PostAsync(
                "",
                authorizationRequestContent,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            AuthorizationToken? authorizationToken = await response
                .Content
                .ReadFromJsonAsync<AuthorizationToken>(cancellationToken);

            return authorizationToken ?? Result.Failure<AuthorizationToken>(AuthenticationErrors.Failed);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<AuthorizationToken>(AuthenticationErrors.Failed);
        }
    }

    /// <summary>
    /// Renews an existing authorization token using the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">
    /// The refresh token used to request a new authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete. Optional.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthorizationToken}"/> containing the renewed <see cref="AuthorizationToken"/> 
    /// if the operation succeeds, or a failure result if it fails.
    /// </returns>
    /// <remarks>
    /// This method sends a request to the authentication server to renew the authorization token.
    /// If the request fails or the response is invalid, a failure result is returned.
    /// </remarks>
    /// <exception cref="HttpRequestException">
    /// Thrown if there is an issue with the HTTP request during the token renewal process.
    /// </exception>
    public async Task<Result<AuthorizationToken>> RenewAuthorizationTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", _keycloakOptions.AuthClientId),
                new("client_secret", _keycloakOptions.AuthClientSecret),
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken)
            };

            using var authorizationRequestContent = new FormUrlEncodedContent(authRequestParameters);

            HttpResponseMessage response = await _httpClient.PostAsync(
                "",
                authorizationRequestContent,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            AuthorizationToken? authorizationToken = await response
                .Content
                .ReadFromJsonAsync<AuthorizationToken>(cancellationToken);

            return authorizationToken ?? Result.Failure<AuthorizationToken>(AuthenticationErrors.AuthorizationTokenRenew);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<AuthorizationToken>(AuthenticationErrors.AuthorizationTokenRenew);
        }
    }
}
