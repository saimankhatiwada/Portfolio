using FluentValidation;

namespace Portfolio.Application.Users.LogInUser;

internal sealed class LoginUserCommandValidator : AbstractValidator<LogInUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Email is invalid.");

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty.")
            .Matches(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage("Password must contain at least one uppercase letter, one digit, one special character, and be at least 8 characters long.");
    }
}
