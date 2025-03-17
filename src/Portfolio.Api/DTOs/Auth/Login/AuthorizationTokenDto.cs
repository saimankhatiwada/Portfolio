namespace Portfolio.Api.DTOs.Auth.Login;

/// <summary>
/// Represents a Data Transfer Object (DTO) for an authorization token.
/// </summary>
/// <remarks>
/// This DTO is used to encapsulate the access and refresh tokens required for authentication and authorization.
/// The <see cref="AccessToken"/> is used to access secured resources, while the <see cref="RefreshToken"/>
/// is used to obtain a new access token when the current one expires.
/// </remarks>
public sealed record AuthorizationTokenDto
{
    /// <summary>
    /// Gets the access token used to authenticate and authorize access to secured resources.
    /// </summary>
    /// <remarks>
    /// The access token is a short-lived token that is included in the headers of requests 
    /// to secured endpoints. It is issued as part of the authentication process and is 
    /// required to access protected resources.
    /// </remarks>
    public required string AccessToken { get; init; }
    /// <summary>
    /// Gets or initializes the refresh token used to obtain a new access token when the current one expires.
    /// </summary>
    /// <remarks>
    /// The refresh token is a secure string that allows the client to request a new <see cref="AccessToken"/> 
    /// without requiring the user to re-authenticate. It is typically issued alongside the access token.
    /// </remarks>
    public required string RefreshToken { get; init; }
}
