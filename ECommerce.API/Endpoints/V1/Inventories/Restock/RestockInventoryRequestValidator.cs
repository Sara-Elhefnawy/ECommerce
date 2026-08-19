using FluentValidation;

namespace ECommerce.API.Endpoints.V1.Inventories.Restock;

public sealed class RestockInventoryRequestValidator : AbstractValidator<RestockInventoryRequest>
{
    public RestockInventoryRequestValidator()
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
