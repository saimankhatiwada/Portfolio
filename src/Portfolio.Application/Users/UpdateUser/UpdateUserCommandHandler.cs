using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Result>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(new UserId(request.Id), cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        
        user.UpdateUser(new FirstName(request.FirstName), new LastName(request.LastName), new Email(request.Email));
        
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
