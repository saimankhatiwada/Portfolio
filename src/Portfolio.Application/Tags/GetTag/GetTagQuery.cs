using Portfolio.Application.Abstractions.Caching;
using Portfolio.Application.Model.Tag;

namespace Portfolio.Application.Tags.GetTag;

public sealed record GetTagQuery(string Id, string? Fields) : ICachedQuery<TagDto>
{
    public string CacheKey => $"{nameof(GetTagQuery)}-{Id}-{Fields}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}
