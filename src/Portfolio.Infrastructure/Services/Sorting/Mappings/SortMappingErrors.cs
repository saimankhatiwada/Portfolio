using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Services.Sorting.Mappings;

public static class SortMappingErrors
{
    public static readonly Error MappingFailed = new(
        "Sort.MappingFailed",
        "The provided sort parameter isn't valid.");
}
