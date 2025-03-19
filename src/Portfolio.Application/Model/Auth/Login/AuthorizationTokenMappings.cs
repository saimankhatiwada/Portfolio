using Portfolio.Application.Abstractions.Authentication;

namespace Portfolio.Application.Model.Auth.Login;

internal static class AuthorizationTokenMappings
{
    public static AuthorizationTokenDto ToDto(AuthorizationToken authorizationToken)
    {
        return new AuthorizationTokenDto()
        {
            AccessToken = authorizationToken.AccessToken,
            RefreshToken = authorizationToken.RefreshToken
        };
    }
}
