using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs.Common;

namespace Portfolio.Api.Users;

public sealed record UsersQueryParameters : AcceptHeaderDto
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public string? Sort { get; init; }
    public string? Fields { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
