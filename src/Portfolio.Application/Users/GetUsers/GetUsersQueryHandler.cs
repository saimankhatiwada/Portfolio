using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.User;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.GetUsers;

/// <summary>
/// Handles the execution of the <see cref="GetUsersQuery"/> to retrieve a read-only list of <see cref="User"/> entities.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IQueryHandler{TQuery, TResponse}"/> interface, where the query is of type <see cref="GetUsersQuery"/>
/// and the response is a <see cref="Result{TResponse}"/> containing a read-only list of <see cref="User"/> entities.
/// </remarks>
internal sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PaginationResult<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    /// <summary>
    /// Processes the specified <see cref="GetUsersQuery"/> to retrieve a result containing a read-only list of <see cref="User"/> entities.
    /// </summary>
    /// <param name="request">The query containing search and sort parameters for retrieving users.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="Result{TValue}"/> 
    /// with a read-only list of <see cref="User"/> entities if successful, or an error if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method interacts with the <see cref="IUserRepository"/> to fetch user data based on the provided query parameters.
    /// </remarks>
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
