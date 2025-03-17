using FluentValidation;

namespace Portfolio.Application.Users.RenewAuthorization;

/// <summary>
/// Validates the <see cref="RenewAuthorizationCommand"/> to ensure it meets the required criteria.
/// </summary>
/// <remarks>
/// This validator enforces rules for the <see cref="RenewAuthorizationCommand"/>, such as ensuring that the
/// <c>RefreshToken</c> property is not empty. It extends the <see cref="AbstractValidator{T}"/> class from FluentValidation
/// to provide a structured approach to validation.
/// </remarks>
internal sealed class RenewAuthorizationCommandValidator : AbstractValidator<RenewAuthorizationCommand>
{
    public RenewAuthorizationCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token cannot be empty.");
    }
}
