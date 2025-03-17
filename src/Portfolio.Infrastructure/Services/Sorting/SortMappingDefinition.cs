namespace Portfolio.Infrastructure.Services.Sorting;

#pragma warning disable S2326
/// <summary>
/// Represents a definition of sort mappings for a specific data source type.
/// </summary>
/// <typeparam name="TSource">
/// The type of the data source for which the sort mappings are defined.
/// </typeparam>
/// <remarks>
/// This class provides a collection of <see cref="SortMapping"/> instances that define
/// how sorting should be applied to the properties of the specified <typeparamref name="TSource"/>.
/// It is typically used in conjunction with services like <see cref="SortMappingProvider"/>
/// to retrieve and validate sorting configurations.
/// </remarks>
public sealed class SortMappingDefinition<TSource> : ISortMappingDefinition
#pragma warning restore S2326
{
    public required SortMapping[] Mappings { get; init; }
}
