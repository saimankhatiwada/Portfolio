namespace Portfolio.Application.Model.User;

public sealed record UpdateUserDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
}
