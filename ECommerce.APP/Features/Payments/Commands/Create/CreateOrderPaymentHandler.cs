using ECommerce.APP.Features.Orders.Queries.GetById;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Payments;
using ECommerce.APP.Settings;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.Extensions.Options;

namespace ECommerce.APP.Features.Payments.Commands.Create;

public sealed class CreateOrderPaymentHandler(
    ICurrentUserService currentUser,
    IRepository<Order> orderRepository,
    IUnitOfWork uow,
    IPaymentService paymentService,
    IOptions<StripeSettings> stripeOptions
    ) : IRequestHandler<CreateOrderPaymentCommand, ResultOfT<PaymentClientSecretResponse>>
{
    public async Task<ResultOfT<PaymentClientSecretResponse>> Handle(CreateOrderPaymentCommand request, CancellationToken ct = default)
    {
        if (currentUser.UserId is null)
            return OrderErrors.Unauthorized;

        var order = await orderRepository.FirstOrDefaultAsync(
            new GetOrderByIdSpecification(request.OrderId, currentUser.UserId.Value, tracking: true),
            ct);

        if (order is null)
            return OrderErrors.NotFound;
        
        var currency = stripeOptions.Value.Currency;
        var amountInSmallestUnit = (long)Math.Round(order.Total * 100m, MidpointRounding.AwayFromZero);

        ResultOfT<PaymentIntentResult> intentResult;

        if (string.IsNullOrWhiteSpace(order.PaymentIntentId))
        {
            intentResult = await paymentService.CreatePaymentIntentAsync(
                amountInSmallestUnit, currency, order.Id, ct);
        }
        else
        {
            intentResult = await paymentService.UpdatePaymentIntentAsync(
                order.PaymentIntentId, amountInSmallestUnit, order.Id, ct);
        }

        if (intentResult.IsFailure)
            return intentResult.Error!;

        var attach = order.AttachPaymentIntent(intentResult.Value.PaymentIntentId);
        if (attach.IsFailure)
            return attach.Error!;

        orderRepository.Update(order);
        await uow.SaveChangesAsync(ct);

        var v = intentResult.Value;
        return ResultOfT<PaymentClientSecretResponse>.Ok(
            new PaymentClientSecretResponse(
                order.Id, v.PaymentIntentId, v.ClientSecret, v.Status, v.PublishableKey));
    }
}
