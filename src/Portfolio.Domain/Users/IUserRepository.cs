using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Domain.Users;

public interface IUserRepository
{
    Task<Result<PaginationResult<User>>> GetAllAsync(string? search, string? sort, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    void Add(User user);
    void Update(User user);
    void Delete(User user);
}
