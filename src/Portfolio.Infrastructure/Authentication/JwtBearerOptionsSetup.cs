using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Portfolio.Infrastructure.Authentication;

/// <summary>
/// Configures the <see cref="JwtBearerOptions"/> for the application by setting up
/// authentication-related properties such as audience, metadata address, issuer, and HTTPS requirements.
/// </summary>
/// <remarks>
/// This class implements <see cref="IConfigureNamedOptions{TOptions}"/> to allow named configuration
/// of <see cref="JwtBearerOptions"/>. It retrieves authentication settings from an instance of
/// <see cref="AuthenticationOptions"/>.
/// </remarks>
internal sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authenticationOptions;

    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    {
        _authenticationOptions = authenticationOptions.Value;
    }
    
    /// <summary>
    /// Configures the provided <see cref="JwtBearerOptions"/> instance using the application's
    /// authentication settings.
    /// </summary>
    /// <param name="options">The <see cref="JwtBearerOptions"/> instance to configure.</param>
    /// <remarks>
    /// This method initializes properties such as <see cref="JwtBearerOptions.Audience"/>,
    /// <see cref="JwtBearerOptions.MetadataAddress"/>, <see cref="JwtBearerOptions.RequireHttpsMetadata"/>,
    /// and <see cref="TokenValidationParameters.ValidIssuer"/> based on the values
    /// defined in the <see cref="AuthenticationOptions"/> configuration.
    /// </remarks>
    public void Configure(JwtBearerOptions options)
    {
        options.Audience = _authenticationOptions.Audience;
        options.MetadataAddress = _authenticationOptions.MetadataUrl;
        options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
        options.TokenValidationParameters.ValidIssuer = _authenticationOptions.Issuer;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}
