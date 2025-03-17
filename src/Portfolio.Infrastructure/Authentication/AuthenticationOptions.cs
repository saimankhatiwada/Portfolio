namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// Represents the configuration options for authentication in the application.
/// </summary>
/// <remarks>
/// This class is used to configure authentication settings such as the audience, metadata URL, 
/// issuer, and whether HTTPS metadata is required. These options are typically populated from 
/// the application's configuration file.
/// </remarks>
public sealed class AuthenticationOptions
{
    public string Audience { get; set; } = string.Empty;

    public string MetadataUrl { get; set; } = string.Empty;

    public bool RequireHttpsMetadata { get; init; }

    public string Issuer { get; set; } = string.Empty;
}
