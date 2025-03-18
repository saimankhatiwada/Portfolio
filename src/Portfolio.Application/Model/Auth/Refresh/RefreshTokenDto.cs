namespace Portfolio.Application.Model.Auth.Refresh;

public sealed record RefreshTokenDto
{
    public required string RefreshToken { get; init; }
}
