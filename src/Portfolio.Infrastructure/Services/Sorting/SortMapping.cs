namespace Portfolio.Infrastructure.Services.Sorting;

/// <summary>
/// Represents a mapping definition for sorting, specifying the field to sort by and the sort direction.
/// </summary>
/// <param name="SortField">
/// The name of the field to sort by. This corresponds to a property in the data source.
/// </param>
/// <param name="Reverse">
/// A boolean value indicating whether the sorting should be in descending order.
/// Defaults to <c>false</c>, meaning ascending order.
/// </param>
public sealed record SortMapping(string SortField, bool Reverse = false);
