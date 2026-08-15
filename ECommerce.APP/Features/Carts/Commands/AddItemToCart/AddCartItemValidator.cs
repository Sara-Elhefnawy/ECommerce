using FluentValidation;

namespace ECommerce.APP.Features.Carts.Commands.AddItemToCart;

public sealed class AddCartItemValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithMessage("Buyer ID is required.")
            .WithErrorCode("Cart.BuyerId.Required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.")
            .WithErrorCode("Cart.ProductId.Required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .WithErrorCode("Cart.Quantity.Invalid");
    }
}
