using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Tags.DeleteTag;

public sealed record DeleteTagCommand(string Id) : ICommand<Result>;
