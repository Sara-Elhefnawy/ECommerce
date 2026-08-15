using FluentValidation;

namespace ECommerce.APP.Features.Carts.Commands.MergeGuestCart;

public sealed class MergeCartValidator : AbstractValidator<MergeCartCommand>
{
    public MergeCartValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithMessage("Buyer ID is required.")
            .WithErrorCode("Cart.BuyerId.Required");

        RuleFor(x => x.AnonymousBuyerId)
            .NotEmpty()
            .WithMessage("Anonymous buyer ID is required.")
            .WithErrorCode("Cart.AnonymousBuyerId.Required");
    }
}
