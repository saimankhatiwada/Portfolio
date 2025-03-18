namespace Portfolio.Application.Model.Auth.Login;

public sealed record AuthorizationTokenDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
