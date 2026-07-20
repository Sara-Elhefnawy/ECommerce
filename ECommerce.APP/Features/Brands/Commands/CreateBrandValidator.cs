using FluentValidation;

namespace ECommerce.APP.Features.Brands.Commands;

public sealed class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Must(name => name.Any(char.IsLetter))
            .WithMessage("Brand name must contain at least one letter.");
    }
}
