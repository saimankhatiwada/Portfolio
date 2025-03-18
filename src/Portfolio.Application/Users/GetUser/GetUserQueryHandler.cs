using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.User;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.GetUser;

internal sealed class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(new UserId(request.Id), cancellationToken);

        return user != null ? UserMappings.ToDto(user) : Result.Failure<UserDto>(UserErrors.NotFound);
    }
}
