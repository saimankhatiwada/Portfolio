namespace Portfolio.Application.Model.User;

internal static class UserMappings
{
    public static UserDto ToDto(Domain.Users.User user)
    {
        return new UserDto()
        {
            Id = user.Id.Value,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Email = user.Email.Value,
            Roles = user.Roles.Select(r => r.Name).ToList()
        };
    }
}
