using Portfolio.Domain.Users;

namespace Portfolio.Infrastructure.Services.Sorting.Mappings;

/// <summary>
/// Provides predefined sort mappings for the <see cref="Portfolio.Domain.Users.User"/> entity.
/// </summary>
/// <remarks>
/// This static class defines a set of sort mappings for the <see cref="Portfolio.Domain.Users.User"/> entity,
/// which can be used to configure sorting behavior in services or repositories.
/// The mappings include properties such as <see cref="Portfolio.Domain.Users.User.FirstName"/>,
/// <see cref="Portfolio.Domain.Users.User.LastName"/>, and <see cref="Portfolio.Domain.Users.User.Email"/>.
/// </remarks>
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
