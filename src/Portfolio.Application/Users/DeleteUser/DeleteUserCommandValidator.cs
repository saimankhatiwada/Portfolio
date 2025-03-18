using FluentValidation;

namespace Portfolio.Application.Users.DeleteUser;

internal sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");
    }
}
