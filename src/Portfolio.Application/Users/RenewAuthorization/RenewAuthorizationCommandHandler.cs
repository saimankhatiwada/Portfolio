using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.RenewAuthorization;

/// <summary>
/// Handles the renewal of authorization tokens by processing a <see cref="RenewAuthorizationCommand"/> 
/// and returning an <see cref="AuthorizationToken"/>.
/// </summary>
/// <remarks>
/// This command handler utilizes the <see cref="IJwtService"/> to renew authorization tokens 
/// based on the provided refresh token. It implements the <see cref="ICommandHandler{TCommand, TResponse}"/> 
/// interface, ensuring compatibility with the MediatR pipeline and encapsulating the result in a 
/// <see cref="Result{TValue}"/> object.
/// </remarks>
internal sealed class RenewAuthorizationCommandHandler : ICommandHandler<RenewAuthorizationCommand, AuthorizationToken>
{
    private readonly IJwtService _jwtService;

    public RenewAuthorizationCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    /// <summary>
    /// Handles the renewal of an authorization token using the provided <see cref="RenewAuthorizationCommand"/>.
    /// </summary>
    /// <param name="request">
    /// The command containing the refresh token required for renewing the authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the renewed <see cref="AuthorizationToken"/> if successful, 
    /// or a failure result with the appropriate error if the operation fails.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the result is a failure and the <see cref="Result{TValue}.Value"/> property is accessed.
    /// </exception>
    /// <remarks>
    /// This method utilizes the <see cref="IJwtService.RenewAuthorizationTokenAsync"/> method to process the 
    /// refresh token and generate a new authorization token. If the operation fails, it returns a failure 
    /// result with the <see cref="UserErrors.RefreshToken"/> error.
    /// </remarks>
    public async Task<Result<AuthorizationToken>> Handle(RenewAuthorizationCommand request, CancellationToken cancellationToken)
    {
        Result<AuthorizationToken> result = await _jwtService.RenewAuthorizationTokenAsync(request.RefreshToken, cancellationToken);

        return result.IsSuccess ? result.Value : Result.Failure<AuthorizationToken>(UserErrors.RefreshToken);
    }
}
