using FluentValidation;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.RegisterUser;

/// <summary>
/// Validates the <see cref="RegisterUserCommand"/> to ensure all required properties meet the specified criteria.
/// </summary>
/// <remarks>
/// This validator enforces rules for the <see cref="RegisterUserCommand"/>, such as:
/// - Ensuring the first name, last name, email, password, and role are not empty.
/// - Validating the email format.
/// - Enforcing password complexity requirements.
/// - Checking the validity of the role.
/// </remarks>
internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .WithMessage("Firstname cannot be empty.");

        RuleFor(c => c.LastName)
            .NotEmpty()
            .WithMessage("Lastname cannot be empty.");

        RuleFor(c => c.Email)
            .EmailAddress()
            .WithMessage("Email is invalid.");

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty.")
            .Matches(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage("Password must contain at least one uppercase letter, one digit, one special character, and be at least 8 characters long.");

        RuleFor(c => c.Role)
            .NotEmpty()
            .WithMessage("Role cannot be empty.")
            .Must(CheckRoleExist)
            .WithMessage("Role is invalid.");
    }

    /// <summary>
    /// Checks if the specified role exists within the predefined roles.
    /// </summary>
    /// <param name="value">The name of the role to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the role exists and is valid; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method uses <see cref="Role.CheckRole"/> to determine if the provided role name corresponds
    /// to a valid predefined role. A role is considered valid if it is not null, empty, or whitespace
    /// and matches one of the predefined roles.
    /// </remarks>
    private static bool CheckRoleExist(string value)
    {
        return !string.IsNullOrWhiteSpace(Role.CheckRole(value).Name);
    }
}
