namespace Portfolio.Application.Exceptions;

/// <summary>
/// Represents an exception that is thrown when validation errors occur during the execution of an application.
/// </summary>
/// <remarks>
/// This exception is typically used to encapsulate one or more validation errors that are detected
/// during the processing of a request or command. The validation errors are accessible through the
/// <see cref="Errors"/> property.
/// </remarks>
/// <example>
/// The following example demonstrates how this exception might be used:
/// <code>
/// var validationErrors = new List&lt;ValidationError&gt;
/// {
///     new ValidationError("PropertyName", "Error message")
/// };
/// throw new ValidationException(validationErrors);
/// </code>
/// </example>
/// <seealso cref="ValidationError"/>
public sealed class ValidationException: Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }

    public IEnumerable<ValidationError> Errors { get; }
}
