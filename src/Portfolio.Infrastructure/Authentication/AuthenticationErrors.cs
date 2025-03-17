using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Authentication;

public static class AuthenticationErrors
{
    public static readonly Error Failed = new(
        "Authentication.Failed",
        "Failed to acquire authorization token due to authentication failure.");

    public static readonly Error AuthorizationTokenRenew = new(
        "Authentication.AuthorizationTokenRenew",
        "Failed to renew authorization token due to authentication failure.");
}
