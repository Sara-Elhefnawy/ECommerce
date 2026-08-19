using FluentValidation;

namespace ECommerce.APP.Features.Orders.Commands.Create;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ShippingAddressId)
            .NotEmpty()
            .WithErrorCode("Order.ShippingAddressId.Required")
            .WithMessage("Shipping address id is required.");

        RuleFor(x => x.DeliveryMethodId)
            .NotEmpty()
            .WithErrorCode("Order.DeliveryMethodId.Required")
            .WithMessage("Delivery method id is required.");
    }
}
