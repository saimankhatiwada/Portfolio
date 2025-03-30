using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Tag;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Tags;

namespace Portfolio.Application.Tags.GetTag;

internal sealed class GetTagQueryHandler : IQueryHandler<GetTagQuery, TagDto>
{
    private readonly ITagRepository _tagRepository;

    public GetTagQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<TagDto>> Handle(GetTagQuery request, CancellationToken cancellationToken)
    {
        Tag? tag = await _tagRepository.GetByIdAsync(new TagId(request.Id), cancellationToken);

        return tag is not null ? TagMappings.ToDto(tag) : Result.Failure<TagDto>(TagErrors.NotFound);
    }
}
