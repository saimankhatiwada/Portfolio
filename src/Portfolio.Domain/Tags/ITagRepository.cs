using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Domain.Tags;

public interface ITagRepository
{
    Task<Result<PaginationResult<Tag>>> GetAllAsync(string? search, string? sort, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Tag?> GetByIdAsync(TagId id, CancellationToken cancellationToken = default);
    void Add(Tag tag);
    void Update(Tag tag);
    void Delete(Tag tag);
}
