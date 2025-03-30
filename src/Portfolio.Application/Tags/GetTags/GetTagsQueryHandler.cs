using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Tag;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Domain.Tags;

namespace Portfolio.Application.Tags.GetTags;

internal sealed class GetTagsQueryHandler : IQueryHandler<GetTagsQuery, PaginationResult<TagDto>>
{
    private readonly ITagRepository _tagRepository;

    public GetTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<PaginationResult<TagDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        Result<PaginationResult<Tag>> result = await _tagRepository
            .GetAllAsync(request.Search, request.Sort, request.Page ?? 1, request.PageSize ?? 10, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<PaginationResult<TagDto>>(result.Error);
        }

        var tagResponses = result.Value.Items.Select(tag => new TagDto()
        {
            Id = tag.Id.Value,
            Name = tag.Name.Value,
            Description = tag.Description?.Value,
        }).ToList();

        var paginatedTagResponses = PaginationResult<TagDto>.Create(
            tagResponses, 
            result.Value.Page,
            result.Value.PageSize, 
            result.Value.TotalCount);

        return paginatedTagResponses;
    }
}
