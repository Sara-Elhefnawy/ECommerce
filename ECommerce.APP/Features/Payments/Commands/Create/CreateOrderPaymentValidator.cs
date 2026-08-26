using FluentValidation;

namespace ECommerce.APP.Features.Payments.Commands.Create;

public sealed class CreateOrderPaymentValidator : AbstractValidator<CreateOrderPaymentCommand>
{
    public CreateOrderPaymentValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithErrorCode("Order.InvalidId")
            .WithMessage("Order id is required.");
    }
}
