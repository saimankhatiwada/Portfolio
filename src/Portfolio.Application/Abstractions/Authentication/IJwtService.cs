using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Authentication;

public interface IJwtService
{
    Task<Result<AuthorizationToken>> GetAuthorizationTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthorizationToken>> RenewAuthorizationTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
