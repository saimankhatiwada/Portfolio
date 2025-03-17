using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.RegisterUser;

/// <summary>
/// Handles the registration of a new user by processing the <see cref="RegisterUserCommand"/>.
/// </summary>
/// <remarks>
/// This class coordinates the user registration process by utilizing the authentication service, 
/// user repository, and unit of work to ensure the user is registered and persisted correctly.
/// </remarks>
/// <seealso cref="Portfolio.Application.Abstractions.Messaging.ICommandHandler{RegisterUserCommand, string}"/>
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

    /// <summary>
    /// Handles the registration of a new user by processing the provided command.
    /// </summary>
    /// <param name="request">The command containing the user's registration details, such as email, first name, last name, password, and role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the unique identifier of the registered user if successful, 
    /// or an error message if the registration fails.
    /// </returns>
    /// <exception cref="ApplicationException">Thrown if the specified role is invalid.</exception>
    /// <remarks>
    /// This method performs the following steps:
    /// 1. Creates a new user instance using the provided details.
    /// 2. Registers the user with the authentication service.
    /// 3. Associates the generated identity ID with the user.
    /// 4. Persists the user to the repository and commits the changes.
    /// </remarks>
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
