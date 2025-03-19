using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Users;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Infrastructure.Services.Sorting;
using Portfolio.Infrastructure.Services.Sorting.Mappings;

namespace Portfolio.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly SortMappingProvider _sortMappingProvider;
    public UserRepository(ApplicationDbContext dbContext, SortMappingProvider sortMappingProvider) : base(dbContext)
    {
        _context = dbContext;
        _sortMappingProvider = sortMappingProvider;
    }

    public override void Add(User user)
    {
        foreach (Role role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(user);
    }

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
