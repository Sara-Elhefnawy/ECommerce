using FluentValidation;

namespace ECommerce.APP.Features.Carts.Commands.ClearCart;

public sealed class ClearCartValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithMessage("Buyer ID is required.")
            .WithErrorCode("Cart.BuyerId.Required");
    }
}
