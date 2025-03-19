﻿using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Authentication;

internal sealed class JwtService : IJwtService
{

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;

    public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

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
