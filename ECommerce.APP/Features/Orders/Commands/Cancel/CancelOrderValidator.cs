using FluentValidation;

namespace ECommerce.APP.Features.Orders.Commands.Cancel;

public sealed class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithErrorCode("Order.OrderId.Required")
            .WithMessage("Order id is required.");
    }
}
