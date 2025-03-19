using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Services.Sorting.Mappings;

internal static class UserSortMapping
{
    public static readonly SortMappingDefinition<User> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(User.FirstName)),
            new SortMapping(nameof(User.LastName)),
            new SortMapping(nameof(User.Email))
        ]
    };
}
