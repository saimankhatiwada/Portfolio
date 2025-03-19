using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users.Events;

namespace Portfolio.Domain.Users;

public sealed class User: Entity<UserId>
{
    private readonly List<Role> _roles = [];

    private User(UserId id, FirstName firstName, LastName lastName, Email email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
    
    private User() {}
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public string IdentityId { get; private set; } = string.Empty;
    public IReadOnlyCollection<Role> Roles => [.. _roles];
    public static User RegisterUser(FirstName firstName, LastName lastName, Email email, Role role)
    {
        var user = new User(UserId.New(), firstName, lastName, email);
        
        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));
        
        user._roles.Add(role);

        return user;
    }

    public void UpdateUser(FirstName firstName, LastName lastName, Email email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
    
    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}
