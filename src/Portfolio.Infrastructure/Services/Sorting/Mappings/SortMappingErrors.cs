using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Services.Sorting.Mappings;

/// <summary>
/// Provides predefined error instances related to sort mapping operations.
/// </summary>
/// <remarks>
/// This class contains static members that represent specific errors encountered
/// during the validation or application of sort mappings. These errors can be used
/// to standardize error handling and messaging across the application.
/// </remarks>
public static class SortMappingErrors
{
    public static readonly Error MappingFailed = new(
        "Sort.MappingFailed",
        "The provided sort parameter isn't valid.");
}
