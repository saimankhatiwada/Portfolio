using FluentValidation;

namespace Portfolio.Application.Tags.DeleteTag;

internal sealed class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");
    }
}
