using Portfolio.Application.Abstractions.Messaging;

namespace Portfolio.Application.Users.RegisterUser;

/// <summary>
/// Represents a command to register a new user in the application.
/// </summary>
/// <remarks>
/// This command encapsulates the necessary information required to register a user, 
/// including their email, first name, last name, password, and role. 
/// It implements the <see cref="ICommand{TResponse}"/> interface, producing a response of type <see cref="string"/>.
/// </remarks>
public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string Role) : ICommand<string>;
