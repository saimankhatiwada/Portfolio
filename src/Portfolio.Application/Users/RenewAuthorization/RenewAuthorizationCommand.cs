using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;

namespace Portfolio.Application.Users.RenewAuthorization;

/// <summary>
/// Represents a command to renew an authorization token using a provided refresh token.
/// </summary>
/// <remarks>
/// This command is used to request a new <see cref="AuthorizationToken"/> by supplying a valid refresh token.
/// It implements the <see cref="ICommand{AuthorizationToken}"/> interface, ensuring compatibility with the application's
/// command handling pipeline.
/// </remarks>
public sealed record RenewAuthorizationCommand(string RefreshToken) : ICommand<AuthorizationToken>;
