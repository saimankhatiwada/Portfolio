using FluentValidation;

namespace Portfolio.Application.Users.LogInUser;

/// <summary>
/// Validates the <see cref="LogInUserCommand"/> to ensure that the provided email and password meet the required criteria.
/// </summary>
/// <remarks>
/// This validator ensures that the email is not empty and follows a valid email format.
/// Additionally, it enforces password requirements, such as containing at least one uppercase letter, one digit,
/// one special character, and being at least 8 characters long.
/// </remarks>
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
