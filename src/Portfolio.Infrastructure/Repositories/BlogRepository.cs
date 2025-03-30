using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Blogs;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Infrastructure.Repositories;

internal sealed class BlogRepository : Repository<Blog, BlogId>, IBlogRepository
{
    public BlogRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Result<PaginationResult<Blog>>> GetAllAsync(string? search, string? sort, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
