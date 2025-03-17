namespace Portfolio.Api.DTOs.Auth.RenewToken;

/// <summary>
/// Represents a Data Transfer Object (DTO) for refreshing an authorization token.
/// </summary>
public sealed record RefreshTokenDto
{
    /// <summary>
    /// Gets or initializes the refresh token used to renew an authorization token.
    /// </summary>
    /// <remarks>
    /// This property is required and must contain a valid refresh token string.
    /// </remarks>
    public required string RefreshToken { get; init; }
}
