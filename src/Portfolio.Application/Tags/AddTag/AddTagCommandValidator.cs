using FluentValidation;

namespace Portfolio.Application.Tags.AddTag;

internal sealed class AddTagCommandValidator : AbstractValidator<AddTagCommand>
{
    public AddTagCommandValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MinimumLength(5)
            .WithMessage("Name must have minimum length of 5.")
            .MaximumLength(50)
            .WithMessage("Name must not exceed the maximum length of 50.");

        When(t => t.Description is not null, () =>
        {
            RuleFor(t => t.Description)
                .MaximumLength(100)
                .WithMessage("Description must not exceed the maximum length of 100.");
        });
    }
}
