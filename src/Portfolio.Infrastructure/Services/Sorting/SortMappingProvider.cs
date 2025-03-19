namespace Portfolio.Infrastructure.Services.Sorting;

internal sealed class SortMappingProvider
{
    private readonly IEnumerable<ISortMappingDefinition> _mappings;

    public SortMappingProvider(IEnumerable<ISortMappingDefinition> mappings)
    {
        _mappings = mappings;
    }

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
