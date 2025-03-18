using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Users;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Infrastructure.Services.Sorting;
using Portfolio.Infrastructure.Services.Sorting.Mappings;

namespace Portfolio.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for managing <see cref="User"/> entities within the application's data context.
/// </summary>
/// <remarks>
/// This sealed class provides specific implementations for handling <see cref="User"/> entities,
/// including adding users and ensuring their associated roles are properly attached to the database context.
/// It extends the generic <see cref="Repository{TEntity, TEntityId}"/> class and implements the <see cref="IUserRepository"/> interface.
/// </remarks>
internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly SortMappingProvider _sortMappingProvider;
    public UserRepository(ApplicationDbContext dbContext, SortMappingProvider sortMappingProvider) : base(dbContext)
    {
        _context = dbContext;
        _sortMappingProvider = sortMappingProvider;
    }

    /// <summary>
    /// Adds a new <see cref="User"/> entity to the database context.
    /// </summary>
    /// <param name="user">The <see cref="User"/> entity to be added.</param>
    /// <remarks>
    /// This method ensures that all associated <see cref="Role"/> entities of the <paramref name="user"/> 
    /// are attached to the database context before adding the user. This guarantees proper tracking 
    /// and persistence of related entities.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="user"/> parameter is <c>null</c>.
    /// </exception>
    public override void Add(User user)
    {
        foreach (Role role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(user);
    }

    /// <summary>
    /// Retrieves a paginated list of users based on the specified search, sort, and field parameters.
    /// </summary>
    /// <param name="search">
    /// An optional search term to filter users by their first or last name. The search term is case-insensitive.
    /// </param>
    /// <param name="sort">
    /// An optional sort expression to order the users. The sort expression must match the defined sort mappings.
    /// </param>
    /// <param name="page">
    /// The page number for pagination. Must be greater than or equal to 1.
    /// </param>
    /// <param name="pageSize">
    /// The number of users to include per page. Must be greater than or equal to 1.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing a <see cref="PaginationResult{TModel}"/> of <see cref="User"/> objects 
    /// if the operation succeeds, or an error result if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method applies search filters, validates sort mappings, and retrieves users from the database in a paginated format.
    /// </remarks>
    public async Task<Result<PaginationResult<User>>> GetAllAsync(string? search, string? sort, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        search ??= search?.Trim().ToLower();

        if (!_sortMappingProvider.ValidateMappings<User>(sort))
        {
            return Result.Failure<PaginationResult<User>>(SortMappingErrors.MappingFailed);
        }

        SortMapping[] sortMappings = _sortMappingProvider.GetMappings<User>();

        IQueryable<User> usersQuery = _context
            .Users
            .Where(u => search == null ||
                        EF.Functions.Like(u.FirstName, $"%{search}%") ||
                        EF.Functions.Like(u.LastName, $"%{search}%"))
            .Include(u => u.Roles)
            .ApplySort(sort, sortMappings);

        int totalCount = await _context.Users.CountAsync(cancellationToken);

        List<User> users = await usersQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var paginatedUsers = PaginationResult<User>.Create(users, page, pageSize, totalCount);

        return paginatedUsers;
    }

    public override async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        User? user = await _context
            .Users
            .Where(u => u.Id == id)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}
