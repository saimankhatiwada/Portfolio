using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Authentication.Models;

/// <summary>
/// Represents a user model used for authentication and user management purposes.
/// </summary>
/// <remarks>
/// This class encapsulates various properties related to a user, such as access permissions, attributes, 
/// roles, credentials, and other metadata required for managing user authentication and identity.
/// </remarks>
internal sealed class UserRepresentationModel
{
    public Dictionary<string, string> Access { get; set; }
    public Dictionary<string, List<string>> Attributes { get; set; }
    public Dictionary<string, string> ClientRoles { get; set; }
    public long? CreatedTimestamp { get; set; }
    public CredentialRepresentationModel[] Credentials { get; set; }
    public string[] DisableableCredentialTypes { get; set; }
    public string Email { get; set; }
    public bool? EmailVerified { get; set; }
    public bool? Enabled { get; set; }
    public string FederationLink { get; set; }
    public string Id { get; set; }
    public string[] Groups { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? NotBefore { get; set; }
    public string Origin { get; set; }
    public string[] RealmRoles { get; set; }
    public string[] RequiredActions { get; set; }
    public string Self { get; set; }
    public string ServiceAccountClientId { get; set; }
    public string Username { get; set; }
    /// <summary>
    /// Creates a new instance of <see cref="UserRepresentationModel"/> from a given <see cref="User"/> domain entity.
    /// </summary>
    /// <param name="user">The <see cref="User"/> domain entity containing user details.</param>
    /// <returns>
    /// A new instance of <see cref="UserRepresentationModel"/> populated with data from the provided <see cref="User"/>.
    /// </returns>
    /// <remarks>
    /// This method maps the properties of the <see cref="User"/> domain entity to the corresponding properties
    /// of the <see cref="UserRepresentationModel"/>. It initializes default values for certain properties
    /// such as <see cref="Enabled"/>, <see cref="EmailVerified"/>, and <see cref="CreatedTimestamp"/>.
    /// </remarks>
    internal static UserRepresentationModel FromUser(User user) =>
        new()
        {
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Email = user.Email.Value,
            Username = user.Email.Value,
            Enabled = true,
            EmailVerified = true,
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Attributes = [],
            RequiredActions = []
        };
}
