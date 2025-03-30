namespace Portfolio.Application.Model.Tag;

public sealed record UpdateTagDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
