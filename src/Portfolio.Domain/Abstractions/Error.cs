namespace Portfolio.Domain.Abstractions;

/// <summary>
/// Initializes a new instance of the <see cref="Error"/> record with the specified
/// error code and corresponding descriptive message.
/// </summary>
/// <param name="Code">A unique identifier representing the specific error.</param>
/// <param name="Message">A concise and informative description of the error.</param>
public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");
}
