using FluentValidation;

namespace Portfolio.Application.Users.UpdateUser;

internal sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("FirstName cannot be empty.");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName cannot be empty.");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Email is invalid.");
    }
}
