using Portfolio.Api.DTOs.Common;

namespace Portfolio.Api.Users;

public sealed record UserQueryParameters : AcceptHeaderDto
{
    public string? Fields { get; init; }
}
