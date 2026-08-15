using FluentValidation;

namespace ECommerce.APP.Features.Carts.Commands.RemoveItem;

public sealed class RemoveCartItemValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithMessage("Buyer ID is required.")
            .WithErrorCode("Cart.BuyerId.Required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.")
            .WithErrorCode("Cart.ProductId.Required");
    }
}
