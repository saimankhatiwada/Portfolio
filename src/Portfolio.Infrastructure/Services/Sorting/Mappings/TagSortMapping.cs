using Portfolio.Domain.Tags;

namespace Portfolio.Infrastructure.Services.Sorting.Mappings;

internal static class TagSortMapping
{
    public static readonly SortMappingDefinition<Tag> SortMapping = new()
    {
        Mappings = 
        [
            new SortMapping(nameof(Tag.Name))
        ]
    };
}
