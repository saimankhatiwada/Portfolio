using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Authentication;

/// <summary>
/// Represents a service for handling JSON Web Token (JWT) operations, 
/// including generating authorization tokens for authentication purposes.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Asynchronously generates an authorization token for a user based on their email and password.
    /// </summary>
    /// <param name="email">The email address of the user attempting to authenticate.</param>
    /// <param name="password">The password of the user attempting to authenticate.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation. 
    /// The result contains a <see cref="Result{TValue}"/> object, which encapsulates an 
    /// <see cref="AuthorizationToken"/> on success or an error on failure.
    /// </returns>
    /// <remarks>
    /// This method is used to authenticate a user and generate a JSON Web Token (JWT) for authorization purposes. 
    /// The resulting <see cref="AuthorizationToken"/> contains both an access token and a refresh token.
    /// </remarks>
    Task<Result<AuthorizationToken>> GetAuthorizationTokenAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renews an existing authorization token using the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">
    /// The refresh token used to obtain a new authorization token. 
    /// This token must be valid and unexpired.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation. 
    /// The result contains a <see cref="Result{TValue}"/> object encapsulating an <see cref="AuthorizationToken"/> 
    /// if the operation is successful, or an error if it fails.
    /// </returns>
    /// <remarks>
    /// This method is intended to be used when the access token has expired, 
    /// allowing the client to obtain a new access token without requiring re-authentication.
    /// </remarks>
    /// <exception cref="HttpRequestException">
    /// Thrown if there is an issue with the network request during the token renewal process.
    /// </exception>
    Task<Result<AuthorizationToken>> RenewAuthorizationTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
