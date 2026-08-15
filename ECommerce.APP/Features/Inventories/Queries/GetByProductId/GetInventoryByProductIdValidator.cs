using FluentValidation;

namespace ECommerce.APP.Features.Inventories.Queries.GetByProductId;

public sealed class GetInventoryByProductIdValidator : AbstractValidator<GetInventoryByProductIdQuery>
{
    public GetInventoryByProductIdValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductID must not be empty.")
            .WithErrorCode("Inventory.ProductId.Required");
    }
}
