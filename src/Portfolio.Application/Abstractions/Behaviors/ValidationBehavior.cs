using FluentValidation;
using MediatR;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Application.Exceptions;

namespace Portfolio.Application.Abstractions.Behaviors;

/// <summary>
/// Implements validation behavior for MediatR commands, ensuring that all validators for the given request type
/// are executed before the request is processed further.
/// </summary>
/// <typeparam name="TRequest">The type of the command request, which must implement <see cref="IBaseCommand"/>.</typeparam>
/// <typeparam name="TResponse">The type of the command response.</typeparam>
/// <remarks>
/// This behavior validates the incoming request using all registered validators for the specific request type.
/// If validation errors are found, a <see cref="ValidationException"/> is thrown containing the details of the errors.
/// Otherwise, the request is passed to the next handler in the pipeline.
/// </remarks>
internal sealed class ValidationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the execution of the validation behavior for a given request and response type.
    /// </summary>
    /// <param name="request">The request object to be validated and processed.</param>
    /// <param name="next">
    /// A delegate representing the next handler in the pipeline. This is invoked if the validation succeeds.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the response of the next handler in the pipeline
    /// if validation succeeds.
    /// </returns>
    /// <exception cref="Portfolio.Application.Exceptions.ValidationException">
    /// Thrown when validation errors are detected in the request. The exception contains details of the validation errors.
    /// </exception>
    /// <remarks>
    /// This method validates the incoming request using all registered validators for the specific request type.
    /// If no validators are registered, the request is passed directly to the next handler.
    /// </remarks>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationErrors = _validators
            .Select(validator => validator.Validate(context))
            .Where(validationResult => validationResult.Errors.Any())
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .ToList();

        if (validationErrors.Any())
        {
            throw new Exceptions.ValidationException(validationErrors);
        }

        return await next();
    }
}
