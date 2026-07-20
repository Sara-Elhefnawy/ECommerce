using FluentValidation;

namespace ECommerce.APP.Features.Types.Commands;

public sealed class CreateTypeValidator : AbstractValidator<CreateTypeCommand>
{
    public CreateTypeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Must(name => name.Any(char.IsLetter))
            .WithMessage("Type name must contain at least one letter.");
    }
}
