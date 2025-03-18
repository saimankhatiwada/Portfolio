using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Domain.Users;

/// <summary>
/// Defines a contract for handling <see cref="User"/> entities within the domain.
/// </summary>
/// <remarks>
/// The <see cref="IUserRepository"/> interface specifies methods for retrieving, adding,
/// updating, and removing <see cref="User"/> entities. It encapsulates data access logic,
/// ensuring separation of concerns.
/// </remarks>
public interface IUserRepository
{
    Task<Result<PaginationResult<User>>> GetAllAsync(string? search, string? sort, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    void Add(User user);
    void Update(User user);
    void Delete(User user);
}
