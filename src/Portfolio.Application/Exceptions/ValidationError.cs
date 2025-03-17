namespace Portfolio.Application.Exceptions;

/// <summary>
/// Represents a validation error that occurs during the validation process.
/// </summary>
/// <param name="PropertyName">
/// The name of the property that caused the validation error.
/// </param>
/// <param name="ErrorMessage">
/// The error message describing the validation issue.
/// </param>
/// <remarks>
/// This record is used to encapsulate details about a specific validation error,
/// including the property name and the associated error message.
/// </remarks>
public sealed record ValidationError(string PropertyName, string ErrorMessage);
