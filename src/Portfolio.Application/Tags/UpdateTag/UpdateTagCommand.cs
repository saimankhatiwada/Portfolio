using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Tags.UpdateTag;

public sealed record UpdateTagCommand(string Id, string Name, string? Description) : ICommand<Result>;
