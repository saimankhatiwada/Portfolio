using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Domain.Tags;
using Portfolio.Infrastructure.Services.Sorting;
using Portfolio.Infrastructure.Services.Sorting.Mappings;

namespace Portfolio.Infrastructure.Repositories;

internal sealed class TagRepository : Repository<Tag, TagId>, ITagRepository
{
    private readonly ApplicationDbContext _context;
    private readonly SortMappingProvider _sortMappingProvider;

    public TagRepository(ApplicationDbContext dbContext, SortMappingProvider sortMappingProvider) : base(dbContext)
    {
        _context = dbContext;
        _sortMappingProvider = sortMappingProvider;
    }

    public async Task<Result<PaginationResult<Tag>>> GetAllAsync(string? search, string? sort, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        search ??= search?.Trim().ToLower();

        if (!_sortMappingProvider.ValidateMappings<Tag>(sort))
        {
            return Result.Failure<PaginationResult<Tag>>(SortMappingErrors.MappingFailed);
        }

        SortMapping[] sortMappings = _sortMappingProvider.GetMappings<Tag>();

        IQueryable<Tag> tagsQuery = _context
            .Tags
            .Where(t => search == null ||
                        EF.Functions.Like(t.Name, $"%{search}%"))
            .ApplySort(sort, sortMappings);

        int totalCount = await _context.Tags.CountAsync(cancellationToken);

        List<Tag> tags = await tagsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var paginatedTags = PaginationResult<Tag>.Create(tags, page, pageSize, totalCount);

        return paginatedTags;
    }
}
