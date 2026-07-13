using ECommerce.APP.Features.Products.Commands;
using FluentValidation;

namespace ECommerce.APP.Features.Products.Validators;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
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
