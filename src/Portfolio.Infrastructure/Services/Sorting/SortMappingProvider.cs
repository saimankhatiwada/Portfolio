namespace Portfolio.Infrastructure.Services.Sorting;

/// <summary>
/// Provides functionality for managing and retrieving sort mappings for specific data source types.
/// </summary>
/// <remarks>
/// This class is responsible for handling sort mapping definitions, allowing retrieval of mappings
/// and validation of sorting configurations for a given data source type. It works in conjunction
/// with implementations of <see cref="ISortMappingDefinition"/> to define and validate sorting rules.
/// </remarks>
internal sealed class SortMappingProvider
{
    private readonly IEnumerable<ISortMappingDefinition> _mappings;

    public SortMappingProvider(IEnumerable<ISortMappingDefinition> mappings)
    {
        _mappings = mappings;
    }

    /// <summary>
    /// Retrieves the sort mappings associated with the specified data source type.
    /// </summary>
    /// <typeparam name="TSource">The type of the data source for which the sort mappings are retrieved.</typeparam>
    /// <returns>
    /// An array of <see cref="SortMapping"/> objects representing the sort mappings for the specified data source type.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no sort mapping is defined for the specified data source type.
    /// </exception>
    public SortMapping[] GetMappings<TSource>()
    {
        SortMappingDefinition<TSource>? sortMappingDefinition = _mappings
            .OfType<SortMappingDefinition<TSource>>()
            .FirstOrDefault();

        if (sortMappingDefinition is null)
        {
            throw new InvalidOperationException(
                $"The mapping for '{typeof(TSource).Name}' isn't defined");
        }

        return sortMappingDefinition.Mappings;
    }

    /// <summary>
    /// Validates the provided sort string against the defined sort mappings for the specified data source type.
    /// </summary>
    /// <typeparam name="TSource">The type of the data source for which the sort mappings are defined.</typeparam>
    /// <param name="sort">
    /// A comma-separated string representing the sort fields and directions (e.g., "FieldName ASC, AnotherField DESC").
    /// </param>
    /// <returns>
    /// <see langword="true"/> if all fields in the sort string are valid according to the defined sort mappings; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method ensures that the fields specified in the sort string are present in the sort mappings
    /// defined for the given data source type. It ignores case and whitespace discrepancies.
    /// </remarks>
    public bool ValidateMappings<TSource>(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return true;
        }

        var sortFields = sort
            .Split(',')
            .Select(f => f.Trim().Split(' ')[0])
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToList();

        SortMapping[] mapping = GetMappings<TSource>();

        return sortFields.All(f => mapping.Any(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase)));
    }
}
