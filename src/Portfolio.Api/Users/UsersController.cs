using System.Dynamic;
using System.Net.Mime;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs.Common;
using Portfolio.Api.MediaTypes;
using Portfolio.Api.Services;
using Portfolio.Api.Utils;
using Portfolio.Application.Model.User;
using Portfolio.Application.Users.GetUser;
using Portfolio.Application.Users.GetUsers;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Domain.Users;
using Portfolio.Infrastructure.Authorization;

namespace Portfolio.Api.Users;

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

    [HttpGet]
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

    [HttpGet("{id}")]
    [HasPermission(Permissions.UsersRead)]
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

        if (result.IsFailure && string.Equals(result.Error.Code, UserErrors.NotFound.Code, StringComparison.OrdinalIgnoreCase))
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

        return result.IsSuccess ? Ok(shapedUserDto) : Problem(
            statusCode: StatusCodes.Status400BadRequest,
            detail: result.Error.Message);
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
            _linkService.Create(nameof(GetUser), "self", HttpMethods.Get, new { id, fields })
        ];

        return links;
    }
}
