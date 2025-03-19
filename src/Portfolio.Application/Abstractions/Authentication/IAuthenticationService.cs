using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<Result<string>> RegisterAsync(User user, string password, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(string identityId, CancellationToken  cancellationToken = default);
}
