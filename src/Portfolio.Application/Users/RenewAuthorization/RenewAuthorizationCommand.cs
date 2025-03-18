using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Auth.Login;

namespace Portfolio.Application.Users.RenewAuthorization;

/// <summary>
/// Represents a command to renew an authorization token using a provided refresh token.
/// </summary>
/// <remarks>
/// This command is used to request a new <see cref="AuthorizationTokenDto"/> by supplying a valid refresh token.
/// It implements the <see cref="ICommand{AuthorizationTokenDto}"/> interface, ensuring compatibility with the application's
/// command handling pipeline.
/// </remarks>
public sealed record RenewAuthorizationCommand(string RefreshToken) : ICommand<AuthorizationTokenDto>;
