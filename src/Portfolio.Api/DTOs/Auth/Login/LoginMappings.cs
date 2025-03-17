using Portfolio.Application.Abstractions.Authentication;

namespace Portfolio.Api.DTOs.Auth.Login;

/// <summary>
/// Provides mapping functionality for converting authentication-related entities 
/// to their corresponding Data Transfer Objects (DTOs).
/// </summary>
/// <remarks>
/// This static class contains methods to facilitate the transformation of domain 
/// authentication models, such as <see cref="AuthorizationToken"/>, into their 
/// DTO representations, such as <see cref="AuthorizationTokenDto"/>. 
/// These mappings are used to ensure a clear separation between the domain and 
/// API layers.
/// </remarks>
internal static class LoginMappings
{
    /// <summary>
    /// Converts an <see cref="AuthorizationToken"/> instance to its corresponding 
    /// Data Transfer Object (DTO), <see cref="AuthorizationTokenDto"/>.
    /// </summary>
    /// <param name="authorizationToken">
    /// The <see cref="AuthorizationToken"/> instance to be converted.
    /// </param>
    /// <returns>
    /// An <see cref="AuthorizationTokenDto"/> that represents the provided 
    /// <see cref="AuthorizationToken"/>.
    /// </returns>
    /// <remarks>
    /// This method is used to map the domain model <see cref="AuthorizationToken"/> 
    /// to its DTO representation, <see cref="AuthorizationTokenDto"/>, ensuring 
    /// separation of concerns between the domain and API layers.
    /// </remarks>
    public static AuthorizationTokenDto ToDto(AuthorizationToken authorizationToken)
    {
        return new AuthorizationTokenDto()
        {
            AccessToken = authorizationToken.AccessToken,
            RefreshToken = authorizationToken.RefreshToken
        };
    }
}
