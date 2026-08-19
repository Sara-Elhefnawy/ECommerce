using FluentValidation;

namespace ECommerce.APP.Features.Orders.Queries.GetById;

public sealed class GetOrderByIdValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithErrorCode("Order.OrderId.Required")
            .WithMessage("Order id is required.");
    }
}
