using Portfolio.Api.Model.Common;

namespace Portfolio.Api.Users;

public sealed record UserQueryParameters : AcceptHeaderDto
{
    /// <summary>
    /// A comma-separated list of fields to include in the response for data shaping.
    /// Use this parameter to request only the necessary fields.
    /// </summary>
    public string? Fields { get; init; }
}
