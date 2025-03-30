using Portfolio.Application.Abstractions.Caching;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Tags;

namespace Portfolio.Application.Tags.DeleteTag;

internal sealed class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand, Result>
{
    private readonly ITagRepository _tagRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTagCommandHandler(ITagRepository tagRepository, ICacheService cacheService, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Result>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        Tag? tag = await _tagRepository.GetByIdAsync(new TagId(request.Id), cancellationToken);

        if (tag is null)
        {
            return Result.Failure<Result>(TagErrors.NotFound);
        }

        _tagRepository.Delete(tag);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
