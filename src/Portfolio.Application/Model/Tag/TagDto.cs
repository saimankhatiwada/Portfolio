﻿namespace Portfolio.Application.Model.Tag;

public sealed record TagDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
