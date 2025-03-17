namespace Portfolio.Api.DTOs.Auth.Login;

/// <summary>
/// Represents the data transfer object for logging in a user.
/// </summary>
/// <remarks>
/// This record is used to encapsulate the user's login credentials, including email and password, 
/// which are required for authentication purposes.
/// </remarks>
public sealed record LoginUserDto
{
    /// <summary>
    /// Gets or sets the email address of the user attempting to log in.
    /// </summary>
    /// <remarks>
    /// This property is required and represents the user's email address, which is used 
    /// as part of the authentication process.
    /// </remarks>
    public required string Email { get; init; }
    /// <summary>
    /// Gets or initializes the password of the user attempting to log in.
    /// </summary>
    /// <remarks>
    /// This property holds the user's password, which is required for authentication purposes.
    /// Ensure that the password is securely handled and transmitted.
    /// </remarks>
    public required string Password { get; init; }
}
