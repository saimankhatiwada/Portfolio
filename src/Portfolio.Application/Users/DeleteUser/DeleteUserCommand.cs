using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Users.DeleteUser;

public sealed record DeleteUserCommand(string Id) : ICommand<Result>;
