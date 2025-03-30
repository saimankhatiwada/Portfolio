using System.Dynamic;
using System.Net.Mime;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.MediaTypes;
using Portfolio.Api.Model.Common;
using Portfolio.Api.Services;
using Portfolio.Api.Utils;
using Portfolio.Application.Model.Tag;
using Portfolio.Application.Tags.AddTag;
using Portfolio.Application.Tags.DeleteTag;
using Portfolio.Application.Tags.GetTag;
using Portfolio.Application.Tags.GetTags;
using Portfolio.Application.Tags.UpdateTag;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Models.Common;
using Portfolio.Domain.Tags;
using Portfolio.Infrastructure.Authorization;

namespace Portfolio.Api.Tags;

/// <summary>
/// Represents endpoints that handles tag-related operations.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("tags")]
[Produces(MediaTypeNames.Application.Json,
    MediaTypeNames.Application.Xml,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1)]
public class TagsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly DataShapingService _dataShapingService;
    private readonly LinkService _linkService;

    public TagsController(ISender sender, DataShapingService dataShapingService, LinkService linkService)
    {
        _sender = sender;
        _dataShapingService = dataShapingService;
        _linkService = linkService;
    }

    /// <summary>
    /// Retrieves a paginated list of tags based on the specified query parameters.
    /// </summary>
    /// <remarks>
    /// This endpoint supports data shaping and pagination. The <c>Fields</c> parameter can be used to specify
    /// the properties to include in the response. If <c>application/vnd.portfolio.hateoas+json</c> is used, HATEOAS links
    /// will be included in the response. This endpoint requires <c>tags:read</c> permission.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType<PaginationResultDto<TagDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.TagsRead)]
    public async Task<IActionResult> GetTags([FromQuery] TagsQueryParameters queryParameters,
        CancellationToken cancellationToken = default)
    {
        if (!_dataShapingService.Validate<Tag>(queryParameters.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{queryParameters.Fields}'");
        }

        var query = new GetTagsQuery(queryParameters.Search, queryParameters.Sort, queryParameters.Fields,
            queryParameters.Page, queryParameters.PageSize);

        Result<PaginationResult<TagDto>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                detail: result.Error.Message);
        }

        var paginationResult = new PaginationResultDto<ExpandoObject>()
        {
            Items = _dataShapingService.ShapeCollectionData(
                result.Value.Items,
                queryParameters.Fields,
                queryParameters.IncludeLinks ? h => CreateLinksForTag(h.Id, queryParameters.Fields) : null),
            Page = result.Value.Page,
            PageSize = result.Value.PageSize,
            TotalCount = result.Value.TotalCount
        };

        if (queryParameters.IncludeLinks)
        {
            paginationResult.Links = CreateLinksForTags(
                queryParameters, 
                paginationResult.HasNextPage,
                paginationResult.HasPreviousPage);
        }

        return Ok(paginationResult);
    }

    /// <summary>
    /// Retrieves a tag by their unique identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint supports data shaping. The <c>Fields</c> parameter can be used to specify
    /// the properties to include in the response. If <c>application/vnd.portfolio.hateoas+json</c> is used, HATEOAS links
    /// will be included in the response. This endpoint requires <c>tags:read-single</c> permission.
    /// </remarks>
    [HttpGet("{id}")]
    [ProducesResponseType<TagDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.TagsReadSingle)]
    public async Task<IActionResult> GetTag(string id, [FromQuery] TagQueryParameters queryParameters,
        CancellationToken cancellationToken = default)
    {
        if (!_dataShapingService.Validate<Tag>(queryParameters.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{queryParameters.Fields}'");
        }

        var query = new GetTagQuery(id, queryParameters.Fields);

        Result<TagDto> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure
            && string.Equals(result.Error.Code, ErrorCodes.Tags.NotFound, StringComparison.OrdinalIgnoreCase))
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: result.Error.Message);
        }

        ExpandoObject shapedTagDto = _dataShapingService.ShapeData(result.Value, queryParameters.Fields);

        if (queryParameters.IncludeLinks)
        {
            ((IDictionary<string, object?>)shapedTagDto)[nameof(ILinksResponseDto.Links)] =
                CreateLinksForTag(id, query.Fields);
        }

        return Ok(shapedTagDto);
    }

    /// <summary>
    /// Add a tag with name and description.
    /// </summary>
    /// <remarks>
    /// This operation requires the <c>tags:add</c> permission.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType<TagDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.TagsAdd)]
    public async Task<ActionResult<TagDto>> AddTag([FromBody] AddTagDto addTagDto, CancellationToken cancellationToken = default)
    {
        var command = new AddTagCommand(addTagDto.Name, addTagDto.Description);

        Result<TagDto> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTag), new { id = result.Value.Id }, result.Value)
            : result.Error.Code switch
            {
                ErrorCodes.Tags.Conflict => Problem(
                    statusCode: StatusCodes.Status409Conflict, 
                    detail: result.Error.Message),
                ErrorCodes.Tags.UserNotFound => Problem(
                    statusCode: StatusCodes.Status404NotFound, 
                    detail: result.Error.Message),
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest, 
                    detail: result.Error.Message)
            };
    }

    /// <summary>
    /// Updates a tag with the specified identifier.
    /// </summary>
    /// <remarks>
    /// This operation requires the <c>tags:update</c> permission.
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.TagsUpdate)]
    public async Task<ActionResult> UpdateTag(string id, [FromBody] UpdateTagDto updateTagDto, CancellationToken cancellationToken = default)
    {
        var command = new UpdateTagCommand(id, updateTagDto.Name, updateTagDto.Description);

        Result<Result> result = await _sender.Send(command, cancellationToken);

        return result.Value.IsSuccess
            ? NoContent()
            : result.Value.Error.Code switch
            {
                ErrorCodes.Tags.Conflict => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    detail: result.Value.Error.Message),
                ErrorCodes.Tags.NotFound => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: result.Value.Error.Message),
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: result.Value.Error.Message)
            };
    }

    /// <summary>
    /// Deletes a tag with the specified identifier.
    /// </summary>
    /// <remarks>
    /// This operation requires the <c>tags:delete</c> permission.
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HasPermission(Permissions.TagsDelete)]
    public async Task<ActionResult> DeleteTag(string id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteTagCommand(id);

        Result<Result> result = await _sender.Send(command, cancellationToken);

        return result.Value.IsSuccess
            ? NoContent()
            : result.Value.Error.Code switch
            {
                ErrorCodes.Tags.NotFound => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: result.Value.Error.Message),
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: result.Value.Error.Message)
            };
    }

    private List<LinkDto> CreateLinksForTag(string id, string? fields)
    {
        List<LinkDto> links =
        [
            _linkService.Create(nameof(GetTag), "self", HttpMethods.Get, new { id, fields }),
            _linkService.Create(nameof(UpdateTag), "update", HttpMethods.Put, new { id }),
            _linkService.Create(nameof(DeleteTag), "delete", HttpMethods.Delete, new { id })
        ];

        return links;
    }

    private List<LinkDto> CreateLinksForTags(TagsQueryParameters queryParameters, bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            _linkService.Create(nameof(GetTags), "self", HttpMethods.Get, new
            {
                q = queryParameters.Search,
                sort = queryParameters.Sort,
                fields = queryParameters.Fields,
                page = queryParameters.Page,
                pageSize = queryParameters.PageSize,
            }),
            _linkService.Create(nameof(AddTag), "add", HttpMethods.Post)
        ];

        if (hasNextPage)
        {
            links.Add(_linkService.Create(nameof(GetTags), "next-page", HttpMethods.Get, new
            {
                q = queryParameters.Search,
                sort = queryParameters.Sort,
                fields = queryParameters.Fields,
                page = queryParameters.Page + 1,
                pageSize = queryParameters.PageSize,
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(_linkService.Create(nameof(GetTags), "previous-page", HttpMethods.Get, new
            {
                q = queryParameters.Search,
                sort = queryParameters.Sort,
                fields = queryParameters.Fields,
                page = queryParameters.Page - 1,
                pageSize = queryParameters.PageSize,
            }));
        }

        return links;
    }
}
