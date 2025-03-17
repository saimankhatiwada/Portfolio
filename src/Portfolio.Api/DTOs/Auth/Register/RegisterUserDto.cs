namespace Portfolio.Api.DTOs.Auth.Register;

/// <summary>
/// Represents the data transfer object used for registering a new user.
/// </summary>
/// <remarks>
/// This DTO contains the necessary information required to create a new user account, 
/// including email, first name, last name, password, and role.
/// </remarks>
public sealed record RegisterUserDto
{
    /// <summary>
    /// Gets or initializes the email address of the user being registered.
    /// </summary>
    /// <remarks>
    /// This property is required and represents the email address that will be associated 
    /// with the new user account. It must be a valid email format.
    /// </remarks>
    public required string Email { get; init; }
    /// <summary>
    /// Gets or initializes the first name of the user.
    /// </summary>
    /// <remarks>
    /// This property represents the user's first name, which is required for the registration process.
    /// </remarks>
    public required string FirstName { get; init; }
    /// <summary>
    /// Gets or sets the last name of the user being registered.
    /// </summary>
    /// <remarks>
    /// This property is required and represents the user's last name, 
    /// which will be used during the registration process.
    /// </remarks>
    public required string LastName { get; init; }
    /// <summary>
    /// Gets or initializes the password for the user being registered.
    /// </summary>
    /// <remarks>
    /// This property holds the password that the user will use to authenticate their account. 
    /// Ensure the password meets the required security standards.
    /// </remarks>
    public required string Password { get; init; }
    /// <summary>
    /// Gets or initializes the role of the user being registered.
    /// </summary>
    /// <remarks>
    /// This property specifies the role assigned to the user during registration, 
    /// which determines their permissions and access level within the system.
    /// </remarks>
    public required string Role { get; init; }
}
