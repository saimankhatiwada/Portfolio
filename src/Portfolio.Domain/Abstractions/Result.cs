using System.Diagnostics.CodeAnalysis;

namespace Portfolio.Domain.Abstractions;

/// <summary>
/// Represents the outcome of an operation, encapsulating both success and failure states.
/// </summary>
/// <remarks>
/// This class provides a consistent way to handle operation results, supporting both success and error scenarios.
/// It includes methods to create successful results and manage errors for failed operations.
/// </remarks>
public class Result
{
    public Result(bool isSuccess, Error error)
    {
        switch (isSuccess)
        {
            case true when error != Error.None:
            case false when error == Error.None:
                throw new InvalidOperationException();
            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    /// <summary>
    /// Creates a <see cref="Result"/> instance representing a successful operation.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="Result"/> indicating the operation completed successfully.
    /// </returns>
    /// <remarks>
    /// Use this method to denote that an operation has succeeded without any errors.
    /// </remarks>
    public static Result Success() => new(true, Error.None);
    /// <summary>
    /// Creates a successful <see cref="Result{TValue}"/> instance that encapsulates the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the successful result.</typeparam>
    /// <param name="value">The value to include in the successful result.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> instance representing a successful operation
    /// with the provided value encapsulated.
    /// </returns>
    /// <remarks>
    /// Use this method to signify that an operation has succeeded
    /// and has produced a valid result without any associated errors.
    /// </remarks>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with the specified error.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> that explains the reason for the failure.</param>
    /// <returns>
    /// A <see cref="Result"/> instance representing an operation that failed due to the provided error.
    /// </returns>
    /// <remarks>
    /// Use this method to signify a failed operation and supply the corresponding error details.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the given <paramref name="error"/> is <see cref="Error.None"/>,
    /// as a valid error is required to represent a failure.
    /// </exception>
    public static Result Failure(Error error) => new(false, error);
    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> instance with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value associated with the result.</typeparam>
    /// <param name="error">The <see cref="Error"/> that provides details about the failure.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> instance representing a failed operation.
    /// </returns>
    /// <remarks>
    /// This method is used to signal a failed operation by supplying a descriptive error
    /// that clarifies the reason for the failure.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the provided error is <see cref="Error.None"/>, as a valid error is required
    /// to represent a failure.
    /// </exception>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> instance based on the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to encapsulate in the result.</typeparam>
    /// <param name="value">The value to evaluate. If <c>null</c>, a failure result is returned.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> indicating success if the value is not <c>null</c>,
    /// or a failure result with an <see cref="Error.NullValue"/> error if the value is <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method evaluates the input value and creates a corresponding result.
    /// Non-<c>null</c> values generate a success result, while <c>null</c> values
    /// produce a failure result with an appropriate error.
    /// </remarks>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

/// <summary>
/// Represents the result of an operation that produces a value, encapsulating both success and failure states.
/// </summary>
/// <typeparam name="TValue">The type of the value produced by the operation.</typeparam>
/// <remarks>
/// This class extends the <see cref="Result"/> class by associating a value with the operation's outcome.
/// It offers methods to create results with a value for successful operations and handle errors for failures.
/// </remarks>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error): base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");
    
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
