namespace Portfolio.Infrastructure.Services.Sorting;

/// <summary>
/// Defines a contract for providing sort mapping definitions, which specify how sorting 
/// should be applied to entities or data sources.
/// </summary>
/// <remarks>
/// Implementations of this interface are used to define mappings between sort fields 
/// and their corresponding properties or behaviors in a data source. These mappings 
/// are utilized by services like <see cref="SortMappingProvider"/> to retrieve and 
/// validate sorting configurations.
/// </remarks>
public interface ISortMappingDefinition;
