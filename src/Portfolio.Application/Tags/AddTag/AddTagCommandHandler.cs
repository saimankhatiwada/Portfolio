using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Exceptions;
using Portfolio.Application.Model.Tag;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Tags;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Tags.AddTag;

internal sealed class AddTagCommandHandler : ICommandHandler<AddTagCommand, TagDto>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public AddTagCommandHandler(ITagRepository tagRepository, IUserRepository userRepository, IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TagDto>> Handle(AddTagCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(new UserId(_userContext.UserId), cancellationToken);

        if (user is null)
        {
            return Result.Failure<TagDto>(TagErrors.UserNotFound);
        }

        var tag = Tag.AddTag(user.Id, new Name(request.Name), request.Description is null
            ? null
            : new Description(request.Description));

        _tagRepository.Add(tag);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Failure<TagDto>(TagErrors.Conflict);
        }

        return TagMappings.ToDto(tag);
    }
}
