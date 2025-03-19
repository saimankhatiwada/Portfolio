using FluentValidation;
using Portfolio.Domain.Users;

namespace Portfolio.Application.Users.RegisterUser;

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
    private static bool CheckRoleExist(string value)
    {
        return !string.IsNullOrWhiteSpace(Role.CheckRole(value).Name);
    }
}
