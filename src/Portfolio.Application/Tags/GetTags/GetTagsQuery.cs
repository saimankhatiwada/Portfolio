using Portfolio.Application.Abstractions.Caching;
using Portfolio.Application.Model.Tag;
using Portfolio.Domain.Models.Common;

namespace Portfolio.Application.Tags.GetTags;

public sealed record GetTagsQuery(string? Search, string? Sort, string? Fields, int? Page, int? PageSize) : ICachedQuery<PaginationResult<TagDto>>
{
    public string CacheKey => $"{nameof(GetTagsQuery)}-{Search}-{Sort}-{Fields}-{Page}-{PageSize}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}
