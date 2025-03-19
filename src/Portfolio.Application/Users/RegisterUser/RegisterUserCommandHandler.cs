using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, string>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterUserCommandHandler(IAuthenticationService authenticationService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.RegisterUser(
            new FirstName(request.FirstName),
            new LastName(request.LastName),
            new Email(request.Email),
            Role.FormRole(request.Role));

        Result<string> identityId = await _authenticationService.RegisterAsync(
            user,
            request.Password,
            cancellationToken);

        if (identityId.IsFailure)
        {
            return Result.Failure<string>(identityId.Error);
        }

        user.SetIdentityId(identityId.Value);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id.Value;
    }
}
