using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Auth.Login;

namespace Portfolio.Application.Users.LogInUser;

public sealed record LogInUserCommand(string Email, string Password) : ICommand<AuthorizationTokenDto>;
