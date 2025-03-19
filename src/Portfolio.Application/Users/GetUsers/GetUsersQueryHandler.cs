using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.User;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.GetUsers;

internal sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PaginationResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<PaginationResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        Result<PaginationResult<User>> result = await _userRepository
            .GetAllAsync(request.Search, request.Sort, request.Page ?? 1, request.PageSize ?? 10, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<PaginationResult<UserDto>>(result.Error);
        }

        var userResponses = result.Value.Items.Select(user => new UserDto()
        {
            Id = user.Id.Value,
            Email = user.Email.Value,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Roles = user.Roles.Select(role => role.Name).ToList()
        }).ToList();

        var paginatedUserResponses = PaginationResult<UserDto>.Create(
            userResponses, 
            result.Value.Page, 
            result.Value.PageSize, 
            result.Value.TotalCount);

        return paginatedUserResponses;
    }
}
