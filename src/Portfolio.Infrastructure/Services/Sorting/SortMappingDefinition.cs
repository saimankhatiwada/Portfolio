namespace Portfolio.Infrastructure.Services.Sorting;

#pragma warning disable S2326
public sealed class SortMappingDefinition<TSource> : ISortMappingDefinition
#pragma warning restore S2326
{
    public required SortMapping[] Mappings { get; init; }
}
