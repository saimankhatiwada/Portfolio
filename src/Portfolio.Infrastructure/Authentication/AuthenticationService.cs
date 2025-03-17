using System.Net.Http.Json;
using System.Net;
using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;
using Portfolio.Infrastructure.Authentication.Models;

namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// Provides authentication services, including user registration and interaction with external authentication systems.
/// </summary>
/// <remarks>
/// This class is an implementation of the <see cref="Portfolio.Application.Abstractions.Authentication.IAuthenticationService"/> interface.
/// It communicates with external authentication providers, such as Keycloak, to handle authentication-related operations.
/// </remarks>
internal sealed class AuthenticationService : IAuthenticationService
{
    private const string PasswordCredentialType = "password";
    private readonly HttpClient _httpClient;
    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    /// <summary>
    /// Registers a new user in the authentication system.
    /// </summary>
    /// <param name="user">The user to be registered.</param>
    /// <param name="password">The password for the new user account.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the unique identifier of the registered user if the operation
    /// succeeds, or an error result if the registration fails.
    /// </returns>
    /// <remarks>
    /// This method communicates with an external authentication provider to create a user account.
    /// It processes specific HTTP response scenarios, such as conflicts or invalid requests, and converts them
    /// into meaningful error results.
    /// </remarks>
    /// <exception cref="HttpRequestException">
    /// Thrown when an HTTP request error occurs during the user registration process.
    /// </exception>
    public async Task<Result<string>> RegisterAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(user);

        userRepresentationModel.Credentials =
        [
            new()
            {
                Value = password,
                Temporary = false,
                Type = PasswordCredentialType
            }
        ];

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "users",
                userRepresentationModel,
                cancellationToken);

            return ExtractIdentityIdFromLocationHeader(response);
        }
        catch (HttpRequestException e)
            when (e.StatusCode == HttpStatusCode.Conflict)
        {
            return Result.Failure<string>(UserErrors.EmailConflict);
        }
        catch (HttpRequestException e)
            when (e.StatusCode == HttpStatusCode.BadRequest)
        {
            return Result.Failure<string>(UserErrors.KeycloakServerError);
        }
    }

    /// <summary>
    /// Extracts the unique identity identifier from the <c>Location</c> header of an HTTP response.
    /// </summary>
    /// <param name="httpResponseMessage">The HTTP response message containing the <c>Location</c> header.</param>
    /// <returns>
    /// A <see cref="string"/> representing the unique identity identifier extracted from the <c>Location</c> header.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <c>Location</c> header is null or does not contain the expected format.
    /// </exception>
    /// <remarks>
    /// This method assumes that the <c>Location</c> header contains a path segment in the format <c>users/{id}</c>, 
    /// where <c>{id}</c> is the unique identity identifier.
    /// </remarks>
    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";

        string? locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery
                                 ?? throw new InvalidOperationException("Location header can't be null");

        int userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        string userIdentityId = locationHeader[
            (userSegmentValueIndex + usersSegmentName.Length)..];

        return userIdentityId;
    }
}
