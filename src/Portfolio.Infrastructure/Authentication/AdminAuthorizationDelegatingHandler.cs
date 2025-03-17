using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Portfolio.Infrastructure.Authentication.Models;

namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// A delegating handler that manages the authorization process for administrative requests
/// by attaching a JWT access token to the request headers.
/// </summary>
/// <remarks>
/// This handler retrieves an administrative authorization token from a Keycloak server
/// and ensures that the token is included in the HTTP request's authorization header.
/// It also validates the success of the HTTP response.
/// </remarks>
/// <seealso cref="DelegatingHandler" />
internal sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly KeycloakOptions _keycloakOptions;
    
    public AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keycloakOptions)
    {
        _keycloakOptions = keycloakOptions.Value;
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler after appending an administrative
    /// JWT access token to the request headers.
    /// </summary>
    /// <param name="request">The HTTP request message to process.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains
    /// the HTTP response message received from the server.
    /// </returns>
    /// <remarks>
    /// This method retrieves an administrative JWT access token from the Keycloak
    /// server configuration and appends it to the request's authorization header.
    /// It ensures the server response indicates success.
    /// </remarks>
    /// <exception cref="HttpRequestException">
    /// Thrown when the HTTP response indicates a failure.
    /// </exception>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AdminAuthorizationToken authorizationToken = await GetAuthorizationToken(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            authorizationToken.AccessToken);

        HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return httpResponseMessage;
    }

    /// <summary>
    /// Retrieves an administrative authorization token from the Keycloak server.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an 
    /// <see cref="AdminAuthorizationToken"/> object, which includes the access token required
    /// for administrative authorization.
    /// </returns>
    /// <exception cref="ApplicationException">
    /// Thrown when the response content cannot be deserialized into an <see cref="AdminAuthorizationToken"/>.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown when the HTTP request to the Keycloak server fails or the response indicates an error.
    /// </exception>
    /// <remarks>
    /// This method sends a POST request to the Keycloak server with the required client credentials
    /// to obtain a JWT access token. The token is used for authorizing administrative requests.
    /// </remarks>
    private async Task<AdminAuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
    {
        var authorizationRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _keycloakOptions.AdminClientId),
            new("client_secret", _keycloakOptions.AdminClientSecret),
            new("scope", "openid email"),
            new("grant_type", "client_credentials")
        };

        var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameters);

        using var authorizationRequest = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(_keycloakOptions.TokenUrl));
        
        authorizationRequest.Content = authorizationRequestContent;

        HttpResponseMessage authorizationResponse = await base.SendAsync(authorizationRequest, cancellationToken);

        authorizationResponse.EnsureSuccessStatusCode();

        return await authorizationResponse.Content.ReadFromJsonAsync<AdminAuthorizationToken>(cancellationToken) ??
               throw new ApplicationException();
    }
}
