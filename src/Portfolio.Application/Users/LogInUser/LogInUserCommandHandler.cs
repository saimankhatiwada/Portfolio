using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.LogInUser;

/// <summary>
/// Handles the <see cref="LogInUserCommand"/> to authenticate a user and generate an <see cref="AuthorizationToken"/>.
/// </summary>
/// <remarks>
/// This class implements the <see cref="ICommandHandler{TCommand, TResponse}"/> interface, 
/// processing user login commands and returning a result encapsulating an authorization token.
/// It utilizes the <see cref="IJwtService"/> to perform authentication and token generation.
/// </remarks>
internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, AuthorizationToken>
{
    private readonly IJwtService _jwtService;

    public LogInUserCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    /// <summary>
    /// Processes the <see cref="LogInUserCommand"/> to authenticate a user and generate an <see cref="AuthorizationToken"/>.
    /// </summary>
    /// <param name="request">The login command containing the user's email and password.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{AuthorizationToken}"/> containing the generated authorization token if authentication is successful, 
    /// or a failure result with an appropriate error if authentication fails.
    /// </returns>
    /// <remarks>
    /// This method utilizes the <see cref="IJwtService"/> to validate the provided credentials and generate an authorization token. 
    /// If the credentials are invalid, it returns a failure result with <see cref="UserErrors.InvalidCredentials"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an attempt is made to access the value of a failure result.
    /// </exception>
    public async Task<Result<AuthorizationToken>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        Result<AuthorizationToken> result = await _jwtService.GetAuthorizationTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return result.IsSuccess ? result.Value : Result.Failure<AuthorizationToken>(UserErrors.InvalidCredentials);
    }
}
