using System.Net.Mime;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs.Auth.Login;
using Portfolio.Api.DTOs.Auth.Register;
using Portfolio.Api.DTOs.Auth.RenewToken;
using Portfolio.Api.MediaTypes;
using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Users.LogInUser;
using Portfolio.Application.Users.RegisterUser;
using Portfolio.Application.Users.RenewAuthorization;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Api.Authentication;

/// <summary>
/// Provides authentication-related endpoints for user login, registration, and token refresh operations.
/// </summary>
/// <remarks>
/// This controller handles user authentication processes, including logging in, registering new users,
/// and refreshing authorization tokens. It supports version 1 of the API and produces JSON and XML responses,
/// including custom media types for version 1.
/// </remarks>s
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
    /// <param name="request">
    /// The login request containing the user's email and password.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// A <see cref="AuthorizationTokenDto"/> if the authentication is successful, returned with a 200 OK status.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// A 400 Bad Request status if the request is invalid.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// A 401 Unauthorized status if authentication fails.
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This endpoint is accessible without authentication and is used to log in users.
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthorizationTokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorizationTokenDto>> LogIn([FromBody] LoginUserDto request, CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request.Email, request.Password);

        Result<AuthorizationToken> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? 
            Ok(LoginMappings.ToDto(result.Value)) : 
            Problem(
                statusCode: StatusCodes.Status401Unauthorized, 
                detail: result.Error.Message);
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="request">
    /// The data required to register a user, including:
    /// <list type="bullet">
    /// <item><description>Email</description></item>
    /// <item><description>First name</description></item>
    /// <item><description>Last name</description></item>
    /// <item><description>Password</description></item>
    /// <item><description>Role</description></item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing:
    /// <list type="bullet">
    /// <item>
    /// <description>A status code of 200 (OK) with the registered user's identifier if the operation is successful.</description>
    /// </item>
    /// <item>
    /// <description>A status code of 409 (Conflict) if there is an email conflict.</description>
    /// </item>
    /// <item>
    /// <description>A status code of 400 (Bad Request) with an error message if the operation fails for other reasons.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This endpoint is accessible without authentication and is used to create a new user account.
    /// </remarks>
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
                "User.EmailConflict" => Problem(
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
    /// <param name="request">The request containing the refresh token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an 
    /// <see cref="AuthorizationTokenDto"/> if the operation is successful, or a problem response 
    /// with a status code of 400 if the operation fails.
    /// </returns>
    /// <remarks>
    /// This endpoint is accessible anonymously and is used to renew the authorization token 
    /// when the current token has expired or is no longer valid.
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<AuthorizationTokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorizationTokenDto>> Refresh([FromBody] RefreshTokenDto request, CancellationToken cancellationToken)
    {
        var command = new RenewAuthorizationCommand(request.RefreshToken);

        Result<AuthorizationToken> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ?
            Ok(LoginMappings.ToDto(result.Value)) :
            Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: result.Error.Message);
    }
}
