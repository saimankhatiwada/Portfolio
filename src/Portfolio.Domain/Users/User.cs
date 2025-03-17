using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Users.Events;

namespace Portfolio.Domain.Users;

/// <summary>
/// Represents a user within the domain, encapsulating core user details and associated behavior.
/// </summary>
/// <remarks>
/// The <see cref="User"/> class serves as a domain entity that defines a user with properties
/// such as first name, last name, email, and roles. It includes functionality for registering
/// new users, managing their roles, and handling domain events related to user actions.
/// </remarks>
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

    /// <summary>
    /// Registers a new user with the specified details and assigns an initial role.
    /// </summary>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="email">The email address of the user.</param>
    /// <param name="role">The initial role to be assigned to the user.</param>
    /// <returns>A new instance of the <see cref="User"/> class representing the registered user.</returns>
    /// <remarks>
    /// This method creates a new user with the provided details, raises a <see cref="UserRegisteredDomainEvent"/> 
    /// to signify the registration, and assigns the specified role to the user.
    /// </remarks>
    public static User RegisterUser(FirstName firstName, LastName lastName, Email email, Role role)
    {
        var user = new User(UserId.New(), firstName, lastName, email);
        
        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));
        
        user._roles.Add(role);

        return user;
    }

    /// <summary>
    /// Sets the identity identifier for the user.
    /// </summary>
    /// <param name="identityId">The identity identifier to associate with the user.</param>
    /// <remarks>
    /// This method assigns the provided identity identifier to the user, allowing the user
    /// to be linked with an external identity system or service.
    /// </remarks>
    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}
