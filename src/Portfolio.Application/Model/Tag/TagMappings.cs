namespace Portfolio.Application.Model.Tag;

internal static class TagMappings
{
    public static TagDto ToDto(Domain.Tags.Tag tag)
    {
        return new TagDto()
        {
            Id = tag.Id.Value,
            Name = tag.Name.Value,
            Description = tag.Description?.Value
        };
    }
}
