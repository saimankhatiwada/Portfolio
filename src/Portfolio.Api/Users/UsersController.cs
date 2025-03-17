using System.Net.Mime;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs.Common;
using Portfolio.Api.DTOs.User;
using Portfolio.Api.MediaTypes;
using Portfolio.Api.Utils;
using Portfolio.Application.Users.GetUsers;
using Portfolio.Application.Users.Shared;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Infrastructure.Authorization;

namespace Portfolio.Api.Users;

[ApiController]
[ApiVersion(1)]
[Route("users")]
[Produces(MediaTypeNames.Application.Json,
    MediaTypeNames.Application.Xml,
    CustomMediaTypeNames.Application.JsonV1)]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;   
    }

    [HttpGet]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetUsers([FromQuery] UsersQueryParameters request, CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery(request.Search, request.Sort, request.Fields, request.Page, request.PageSize);

        Result<PaginationResult<UserResponse>> result = await _sender.Send(query, cancellationToken);
        
        var usersDto = result.Value
            .Items
            .Select(user => new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.Roles
            })
            .ToList();

        var paginatedUserDto = PaginationResultDto<UserDto>
            .Create(
                usersDto, 
                result.Value.Page, 
                result.Value.PageSize,
                result.Value.TotalCount);
        
        return result.IsSuccess ? Ok(paginatedUserDto) : BadRequest(result);
    }
}
