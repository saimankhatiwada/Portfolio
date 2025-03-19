using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(string Id, string FirstName, string LastName, string Email) : ICommand<Result>;
