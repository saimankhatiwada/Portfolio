using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Model.Common;

namespace Portfolio.Api.Users;

public sealed record UsersQueryParameters : AcceptHeaderDto
{
    /// <summary>
    /// Search applies only to firstName and lastName.
    /// </summary>
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

    /// <summary>
    /// Specifies the field with sort order to apply.
    /// Example: <c>firstName DSC, lastName, email</c>
    /// </summary>
    public string? Sort { get; init; }

    /// <summary>
    /// A comma-separated list of fields to include in the response for data shaping.
    /// Use this parameter to request only the necessary fields.
    /// </summary>
    public string? Fields { get; init; }

    /// <summary>
    /// The current page number.
    /// Default is <c>1</c>
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// The number of records per page.
    /// Default is <c>10</c>
    /// </summary>
    public int PageSize { get; init; } = 10;
}
