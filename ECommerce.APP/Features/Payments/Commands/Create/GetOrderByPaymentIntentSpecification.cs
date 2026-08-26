using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Payments.Commands.Create;

public sealed class GetOrderByPaymentIntentSpecification : Specification<Order>
{
    public GetOrderByPaymentIntentSpecification(string paymentIntentId)
    {
        Query.Where(o => o.PaymentIntentId == paymentIntentId)
            .AsTracking();
    }
}
