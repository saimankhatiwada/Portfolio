using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Exceptions;

namespace Portfolio.Api.Middleware;

/// <summary>
/// Middleware for handling exceptions that occur during the request pipeline execution.
/// </summary>
/// <remarks>
/// This middleware intercepts unhandled exceptions, logs them, and returns a standardized error response
/// to the client in the form of a <see cref="ProblemDetails"/> object. It ensures that the application
/// provides meaningful error information while maintaining a consistent response structure.
/// </remarks>
/// <example>
/// To use this middleware, add it to the application's request pipeline:
/// <code>
/// var builder = WebApplication.CreateBuilder(args);
/// var app = builder.Build();
/// app.UseMiddleware&lt;ExceptionHandlingMiddleware&gt;();
/// app.Run();
/// </code>
/// </example>
internal sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes an HTTP request and handles any exceptions that occur during the execution of the request pipeline.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method intercepts unhandled exceptions, logs them, and generates a standardized error response
    /// in the form of a <see cref="ProblemDetails"/> object. It ensures that the client receives meaningful
    /// error information while maintaining a consistent response structure.
    /// </remarks>
    /// <exception cref="ValidationException">
    /// Thrown when a validation error occurs. The response will include details about the validation errors.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when an unexpected error occurs. The response will include a generic server error message.
    /// </exception>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            ExceptionDetails exceptionDetails = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail,
            };

            if (exceptionDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }

            context.Response.StatusCode = exceptionDetails.Status;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    /// <summary>
    /// Maps an exception to a structured <see cref="ExceptionDetails"/> object containing relevant error information.
    /// </summary>
    /// <param name="exception">The exception to be mapped.</param>
    /// <returns>
    /// An <see cref="ExceptionDetails"/> object that encapsulates the HTTP status code, error type, title, 
    /// detailed message, and optional validation errors associated with the exception.
    /// </returns>
    /// <remarks>
    /// This method categorizes exceptions into specific types, such as validation errors or server errors, 
    /// and provides a consistent structure for error handling and response generation.
    /// </remarks>
    /// <example>
    /// For a <see cref="ValidationException"/>, the method returns:
    /// <code>
    /// new ExceptionDetails(
    ///     StatusCodes.Status400BadRequest,
    ///     "ValidationFailure",
    ///     "Validation error",
    ///     "One or more validation errors has occurred",
    ///     validationException.Errors)
    /// </code>
    /// For other exceptions, it returns:
    /// <code>
    /// new ExceptionDetails(
    ///     StatusCodes.Status500InternalServerError,
    ///     "ServerError",
    ///     "Server error",
    ///     "An unexpected error has occurred",
    ///     null)
    /// </code>
    /// </example>
    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "ValidationFailure",
                "Validation error",
                "One or more validation errors has occurred",
                validationException.Errors),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServerError",
                "Server error",
                "An unexpected error has occurred",
                null)
        };
    }

    /// <summary>
    /// Represents the details of an exception, including its HTTP status code, type, title, 
    /// detailed message, and optional validation errors.
    /// </summary>
    /// <remarks>
    /// This record is used to encapsulate structured information about exceptions for consistent 
    /// error handling and response generation in the middleware.
    /// </remarks>
    private sealed record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}
