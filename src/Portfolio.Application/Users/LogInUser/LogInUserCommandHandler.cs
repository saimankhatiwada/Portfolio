using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Model.Auth.Login;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.LogInUser;

internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, AuthorizationTokenDto>
{
    private readonly IJwtService _jwtService;
    public LogInUserCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<Result<AuthorizationTokenDto>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        Result<AuthorizationToken> result = await _jwtService.GetAuthorizationTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return result.IsSuccess 
            ? AuthorizationTokenMappings.ToDto(result.Value) 
            : Result.Failure<AuthorizationTokenDto>(UserErrors.InvalidCredentials);
    }
}
