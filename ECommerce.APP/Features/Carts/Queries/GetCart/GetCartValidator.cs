using FluentValidation;

namespace ECommerce.APP.Features.Carts.Queries.GetCart;

public sealed class GetCartValidator : AbstractValidator<GetCartQuery>
{
    public GetCartValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithMessage("Buyer ID is required.")
            .WithErrorCode("Cart.BuyerId.Required");
    }
}
