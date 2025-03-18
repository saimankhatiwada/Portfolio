using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Auth.Login;
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
internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, AuthorizationTokenDto>
{
    private readonly IJwtService _jwtService;

    public LogInUserCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    /// <summary>
    /// Handles the <see cref="LogInUserCommand"/> to authenticate a user and generate an <see cref="AuthorizationTokenDto"/>.
    /// </summary>
    /// <param name="request">The command containing the user's email and password for authentication.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{AuthorizationTokenDto}"/> containing the generated authorization token if authentication is successful, 
    /// or a failure result with an appropriate error if authentication fails.
    /// </returns>
    /// <remarks>
    /// This method uses the <see cref="IJwtService"/> to validate the provided credentials and generate an authorization token. 
    /// If the credentials are invalid, it returns a failure result with <see cref="UserErrors.InvalidCredentials"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an attempt is made to access the value of a failure result.
    /// </exception>
    public async Task<Result<AuthorizationTokenDto>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        Result<AuthorizationToken> result = await _jwtService.GetAuthorizationTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return result.IsSuccess 
            ? AuthorizationTokenMappings.ToDto(result.Value) 
            : Result.Failure<AuthorizationTokenDto>(UserErrors.InvalidCredentials);
    }
}
