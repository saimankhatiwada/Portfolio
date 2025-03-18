using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Auth.Login;

namespace Portfolio.Application.Users.LogInUser;

/// <summary>
/// Represents a command to log in a user using their email and password.
/// </summary>
/// <remarks>
/// This command is used to authenticate a user and retrieve an <see cref="AuthorizationTokenDto"/> 
/// containing the necessary tokens for accessing secured resources.
/// It implements the <see cref="ICommand{AuthorizationTokenDto}"/> interface, enabling it to be processed
/// by a corresponding command handler.
/// </remarks>
public sealed record LogInUserCommand(string Email, string Password) : ICommand<AuthorizationTokenDto>;
