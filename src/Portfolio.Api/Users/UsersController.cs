using System.Dynamic;
using System.Net.Mime;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.MediaTypes;
using Portfolio.Api.Model.Common;
using Portfolio.Api.Services;
using Portfolio.Api.Utils;
using Portfolio.Application.Model.User;
using Portfolio.Application.Users.DeleteUser;
using Portfolio.Application.Users.GetUser;
using Portfolio.Application.Users.GetUsers;
using Portfolio.Application.Users.UpdateUser;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Domain.Users;
using Portfolio.Infrastructure.Authorization;

namespace Portfolio.Api.Users;

/// <summary>
/// Represents a controller that handles user-related operations in the API.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("users")]
[Produces(MediaTypeNames.Application.Json,
    MediaTypeNames.Application.Xml,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1)]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly DataShapingService _dataShapingService;
    private readonly LinkService _linkService;
    public UsersController(ISender sender, DataShapingService dataShapingService, LinkService linkService)
    {
        _sender = sender;
        _dataShapingService = dataShapingService;
        _linkService = linkService;
    }

    /// <summary>
    /// Retrieves a paginated list of users based on the specified query parameters.
    /// </summary>
    /// <remarks>
    /// This endpoint supports data shaping and pagination. The <c>Fields</c> parameter can be used to specify
    /// the properties to include in the response. If <c>application/vnd.portfolio.hateoas+json</c> is used, HATEOAS links
    /// will be included in the response. This endpoint requires <c>users:read</c> permission.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType<PaginationResultDto<UserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetUsers([FromQuery] UsersQueryParameters request, CancellationToken cancellationToken)
    {
        if (!_dataShapingService.Validate<User>(request.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{request.Fields}'");
        }
        
        var query = new GetUsersQuery(request.Search, request.Sort, request.Fields, request.Page, request.PageSize);

        Result<PaginationResult<UserDto>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                detail: result.Error.Message);
        }
        
        var paginationResult = new PaginationResultDto<ExpandoObject>
        {
            Items = _dataShapingService.ShapeCollectionData(
                result.Value.Items,
                request.Fields,
                request.IncludeLinks ? h => CreateLinksForUser(h.Id, request.Fields) : null),
            Page = result.Value.Page,
            PageSize = result.Value.PageSize,
            TotalCount = result.Value.TotalCount
        };
        
        if (request.IncludeLinks)
        {
            paginationResult.Links = CreateLinksForUsers(
                request,
                paginationResult.HasNextPage,
                paginationResult.HasPreviousPage);
        }

        return Ok(paginationResult);

    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint supports data shaping. The <c>Fields</c> parameter can be used to specify
    /// the properties to include in the response. If <c>application/vnd.portfolio.hateoas+json</c> is used, HATEOAS links
    /// will be included in the response. This endpoint requires <c>users:read-single</c> permission.
    /// </remarks>
    [HttpGet("{id}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.UsersReadSingle)]
#pragma warning disable CA1721
    public async Task<IActionResult> GetUser(string id, [FromQuery] UserQueryParameters queryParameters, CancellationToken cancellationToken)
#pragma warning restore CA1721
    {
        if (!_dataShapingService.Validate<User>(queryParameters.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{queryParameters.Fields}'");
        }
        
        var query = new GetUserQuery(id, queryParameters.Fields);

        Result<UserDto> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure 
            && string.Equals(result.Error.Code, UserErrors.NotFound.Code, StringComparison.OrdinalIgnoreCase))
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: result.Error.Message);
        }

        ExpandoObject shapedUserDto = _dataShapingService.ShapeData(result.Value, queryParameters.Fields);
        
        if (queryParameters.IncludeLinks)
        {
            ((IDictionary<string, object?>)shapedUserDto)[nameof(ILinksResponseDto.Links)] =
                CreateLinksForUser(id, query.Fields);
        }

        return Ok(shapedUserDto);
    }

    /// <summary>
    /// Updates a user with the specified identifier.
    /// </summary>
    /// <remarks>
    /// This operation requires the <c>users:update</c> permission.
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.UsersUpdate)]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserCommand(id, updateUserDto.FirstName, updateUserDto.LastName, updateUserDto.Email);

        Result<Result> result = await _sender.Send(command, cancellationToken);

        return result.Value.IsSuccess
            ? NoContent()
            : result.Value.Error.Code switch
            {
                "User.NotFound" => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: result.Value.Error.Message),
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: result.Value.Error.Message)
            };
    }

    /// <summary>
    /// Deletes a user with the specified identifier.
    /// </summary>
    /// <remarks>
    /// This operation requires the <c>users:delete</c> permission.
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.UsersDelete)]
    public async Task<ActionResult> DeleteUser(string id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserCommand(id);

        Result<Result> result = await _sender.Send(command, cancellationToken);

        return result.Value.IsSuccess
            ? NoContent()
            : result.Value.Error.Code switch
            {
                "User.NotFound" => Problem(
                    statusCode: StatusCodes.Status404NotFound, 
                    detail: result.Value.Error.Message),
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest, 
                    detail: result.Value.Error.Message)
            };
    }

    private List<LinkDto> CreateLinksForUsers(UsersQueryParameters usersQueryParameters, bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            _linkService.Create(nameof(GetUsers), "self", HttpMethods.Get, new
            {
                q = usersQueryParameters.Search,
                sort = usersQueryParameters.Sort,
                fields = usersQueryParameters.Fields,
                page = usersQueryParameters.Page,
                pageSize = usersQueryParameters.PageSize,
            })
        ];

        if (hasNextPage)
        {
            links.Add(_linkService.Create(nameof(GetUsers), "next-page", HttpMethods.Get, new
            {
                q = usersQueryParameters.Search,
                sort = usersQueryParameters.Sort,
                fields = usersQueryParameters.Fields,
                page = usersQueryParameters.Page,
                pageSize = usersQueryParameters.PageSize,
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(_linkService.Create(nameof(GetUsers), "previous-page", HttpMethods.Get, new
            {
                q = usersQueryParameters.Search,
                sort = usersQueryParameters.Sort,
                fields = usersQueryParameters.Fields,
                page = usersQueryParameters.Page,
                pageSize = usersQueryParameters.PageSize,
            }));
        }

        return links;
    }

    private List<LinkDto> CreateLinksForUser(string id, string? fields)
    {
        List<LinkDto> links =
        [
            _linkService.Create(nameof(GetUser), "self", HttpMethods.Get, new { id, fields }),
            _linkService.Create(nameof(UpdateUser), "update", HttpMethods.Put, new { id }),
            _linkService.Create(nameof(DeleteUser), "delete", HttpMethods.Delete, new { id })
        ];

        return links;
    }
}
