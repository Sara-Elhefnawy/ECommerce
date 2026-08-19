using FluentValidation;

namespace ECommerce.APP.Features.Inventories.Commands.Restock;

public sealed class RestockInventoryValidator : AbstractValidator<RestockInventoryCommand>
{
    public RestockInventoryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithErrorCode("Inventory.ProductId.Required")
            .WithMessage("Product id is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithErrorCode("Inventory.Quantity.Invalid")
            .WithMessage("Quantity must be greater than zero.");
    }
}
