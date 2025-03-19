using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Auth.Login;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.RenewAuthorization;

internal sealed class RenewAuthorizationCommandHandler : ICommandHandler<RenewAuthorizationCommand, AuthorizationTokenDto>
{
    private readonly IJwtService _jwtService;

    public RenewAuthorizationCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    public async Task<Result<AuthorizationTokenDto>> Handle(RenewAuthorizationCommand request, CancellationToken cancellationToken)
    {
        Result<AuthorizationToken> result = await _jwtService.RenewAuthorizationTokenAsync(request.RefreshToken, cancellationToken);

        return result.IsSuccess 
            ? AuthorizationTokenMappings.ToDto(result.Value) 
            : Result.Failure<AuthorizationTokenDto>(UserErrors.RefreshToken);
    }
}
