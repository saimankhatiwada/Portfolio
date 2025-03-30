using System.Net.Mime;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.MediaTypes;
using Portfolio.Api.Utils;
using Portfolio.Application.Model.Auth.Login;
using Portfolio.Application.Model.Auth.Refresh;
using Portfolio.Application.Model.Auth.Register;
using Portfolio.Application.Users.LogInUser;
using Portfolio.Application.Users.RegisterUser;
using Portfolio.Application.Users.RenewAuthorization;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Api.Authentication;

/// <summary>
/// Provides authentication-related endpoints for user login, registration, and token refresh operations.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("auth")]
[Produces(MediaTypeNames.Application.Json,
    MediaTypeNames.Application.Xml,
    CustomMediaTypeNames.Application.JsonV1)]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Authenticates a user and generates an authorization token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthorizationTokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorizationTokenDto>> LogIn([FromBody] LoginUserDto request, CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request.Email, request.Password);

        Result<AuthorizationTokenDto> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? 
            Ok(result.Value) : 
            Problem(
                statusCode: StatusCodes.Status401Unauthorized, 
                detail: result.Error.Message);
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password,
            request.Role);

        Result<string> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.Code switch
            {
                ErrorCodes.Users.EmailConflict => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    detail: result.Error.Message),
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: result.Error.Message)
            };
    }

    /// <summary>
    /// Refreshes the authorization token using the provided refresh token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<AuthorizationTokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorizationTokenDto>> Refresh([FromBody] RefreshTokenDto request, CancellationToken cancellationToken)
    {
        var command = new RenewAuthorizationCommand(request.RefreshToken);

        Result<AuthorizationTokenDto> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ?
            Ok(result.Value) :
            Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: result.Error.Message);
    }
}
