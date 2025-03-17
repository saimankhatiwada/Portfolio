namespace Portfolio.Application.Users.Shared;

public sealed class UserResponse
{
    public string Id { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public List<string> Roles { get; init; }
}
