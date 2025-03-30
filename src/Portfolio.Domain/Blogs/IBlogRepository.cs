using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Domain.Blogs;

public interface IBlogRepository
{
    Task<Result<PaginationResult<Blog>>> GetAllAsync(string? search, string? sort, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Blog?> GetByIdAsync(BlogId id, CancellationToken cancellationToken = default);
    void Add(Blog blog);
    void Update(Blog blog);
    void Delete(Blog blog);
}
