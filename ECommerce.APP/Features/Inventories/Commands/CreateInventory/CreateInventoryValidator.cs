using FluentValidation;

namespace ECommerce.APP.Features.Inventories.Commands.CreateInventory;

public sealed class CreateInventoryValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.")
            .WithErrorCode("Inventory.ProductId.Required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Inventory quantity must be greater than zero.")
            .WithErrorCode("Inventory.Quantity.Invalid");
    }
}
