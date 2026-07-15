using FluentValidation;

namespace ECommerce.APP.Features.Products.Commands;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Must(name => name.Any(char.IsLetter))
            .WithMessage("Product name must contain at least one letter.");

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.BrandName)
            .NotEmpty();

        RuleFor(x => x.TypeName)
            .NotEmpty();
    }
}
