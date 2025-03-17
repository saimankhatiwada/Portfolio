using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Abstractions.Authentication;

/// <summary>
/// Represents a service responsible for handling authentication-related operations.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user with the provided details and password.
    /// </summary>
    /// <param name="user">The <see cref="User"/> instance containing the user's details.</param>
    /// <param name="password">The password to associate with the user.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a
    /// <see cref="Result{TValue}"/> with the user's identity ID as a string if the operation succeeds.
    /// </returns>
    /// <remarks>
    /// This method handles the registration of a new user, including validation and persistence.
    /// It returns a result encapsulating the success or failure of the operation.
    /// </remarks>
    Task<Result<string>> RegisterAsync(User user, string password, CancellationToken cancellationToken = default);
}
