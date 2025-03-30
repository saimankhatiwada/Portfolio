namespace Portfolio.Application.Model.Tag;

public sealed record AddTagDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
