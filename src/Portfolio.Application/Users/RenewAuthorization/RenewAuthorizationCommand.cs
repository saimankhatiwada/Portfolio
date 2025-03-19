using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Auth.Login;

namespace Portfolio.Application.Users.RenewAuthorization;

public sealed record RenewAuthorizationCommand(string RefreshToken) : ICommand<AuthorizationTokenDto>;
