using Portfolio.Application.Abstractions.Messaging;

namespace Portfolio.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string Role) : ICommand<string>;
