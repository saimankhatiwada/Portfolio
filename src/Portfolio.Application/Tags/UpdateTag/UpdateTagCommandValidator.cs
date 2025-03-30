using FluentValidation;

namespace Portfolio.Application.Tags.UpdateTag;

internal sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");

        RuleFor(t => t.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MinimumLength(5)
            .WithMessage("Name must have a minimum length of 5.")
            .MaximumLength(50)
            .WithMessage("Name must not exceed maximum length of 50.");

        When(t => t.Description is not null, () =>
        {
            RuleFor(t => t.Description)
                .MaximumLength(100)
                .WithMessage("Description must not exceed maximum length of 100.");
        });
    }
}
