using ECommerce.APP.Features.Orders.Enums;
using FluentValidation;

namespace ECommerce.API.Endpoints.V1.Orders.GetUserOrders;

public sealed class GetCurrentUserOrdersRequestValidator : AbstractValidator<GetCurrentUserOrdersRequest>
{
    public GetCurrentUserOrdersRequestValidator()
    {
        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrWhiteSpace(x) ||
                       Enum.TryParse<OrderSortType>(x, true, out _))
            .WithMessage($"SortBy must be one of: {string.Join(", ", Enum.GetNames<OrderSortType>())}");
    }
}
