using System.Linq.Dynamic.Core;

namespace Portfolio.Infrastructure.Services.Sorting;

/// <summary>
/// Provides extension methods for applying sorting to <see cref="IQueryable{T}"/> collections.
/// </summary>
/// <remarks>
/// This class includes methods that allow dynamic sorting of queryable data sources based on
/// specified sort fields and mappings. It is designed to work with LINQ queries and supports
/// custom sort field mappings and default ordering.
/// </remarks>
internal static class QueryableExtensions
{
    /// <summary>
    /// Applies sorting to the specified <see cref="IQueryable{T}"/> based on the provided sort string and mappings.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection to apply sorting to.</param>
    /// <param name="sort">
    /// A comma-separated string specifying the sort fields and directions. 
    /// Each field can be optionally prefixed with a '-' to indicate descending order.
    /// </param>
    /// <param name="mappings">
    /// An array of <see cref="SortMapping"/> objects that define the mapping between sort fields
    /// and their corresponding properties in the data source.
    /// </param>
    /// <param name="defaultOrderBy">
    /// The default field to sort by if no sort string is provided. Defaults to "Id".
    /// </param>
    /// <returns>
    /// A sorted <see cref="IQueryable{T}"/> collection based on the specified sort string and mappings.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a sort field in the <paramref name="sort"/> string does not have a corresponding mapping
    /// in the <paramref name="mappings"/> array.
    /// </exception>
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        string? sort,
        SortMapping[] mappings,
        string defaultOrderBy = "Id")
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(defaultOrderBy);
        }

        string[] sortFields = sort.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        var orderByParts = new List<string>();
        foreach (string field in sortFields)
        {
            (string sortField, bool isDescending) = ParseSortField(field);

            SortMapping mapping = mappings.First(m =>
                m.SortField.Equals(sortField, StringComparison.OrdinalIgnoreCase));

            string direction = (isDescending, mapping.Reverse) switch
            {
                (false, false) => "ASC",
                (false, true) => "DESC",
                (true, false) => "DESC",
                (true, true) => "ASC"
            };

            orderByParts.Add($"{mapping.SortField} {direction}");
        }

        string orderBy = string.Join(",", orderByParts);

        return query.OrderBy(orderBy);
    }

    /// <summary>
    /// Parses a sort field string to extract the field name and sort direction.
    /// </summary>
    /// <param name="field">
    /// The sort field string to parse. The string can optionally include a direction indicator
    /// (e.g., "FieldName desc" for descending order).
    /// </param>
    /// <returns>
    /// A tuple containing the parsed sort field name and a boolean indicating whether the sort direction
    /// is descending. The first item of the tuple is the field name, and the second item is <c>true</c>
    /// if the sort direction is descending; otherwise, <c>false</c>.
    /// </returns>
    private static (string SortField, bool IsDescending) ParseSortField(string field)
    {
        string[] parts = field.Split(' ');
        string sortField = parts[0];
        bool isDescending = parts.Length > 1 &&
                            parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return (sortField, isDescending);
    }
}
