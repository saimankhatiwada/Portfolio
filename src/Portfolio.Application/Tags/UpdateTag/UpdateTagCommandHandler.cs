using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Exceptions;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Tags;

namespace Portfolio.Application.Tags.UpdateTag;

internal sealed class UpdateTagCommandHandler : ICommandHandler<UpdateTagCommand, Result>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Result>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        Tag? tag = await _tagRepository.GetByIdAsync(new TagId(request.Id), cancellationToken);

        if (tag is null)
        {
            return Result.Failure(TagErrors.NotFound);
        }

        tag.UpdateTag(new Name(request.Name), request.Description is null 
            ? null 
            : new Description(request.Description));

        _tagRepository.Update(tag);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Failure(TagErrors.Conflict);
        }

        return Result.Success();
    }
}
