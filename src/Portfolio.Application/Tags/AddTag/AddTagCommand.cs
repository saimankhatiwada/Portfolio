using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Tag;

namespace Portfolio.Application.Tags.AddTag;

public sealed record AddTagCommand(string Name, string? Description) : ICommand<TagDto>;
