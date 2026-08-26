using ECommerce.APP.Features.Payments.Commands.Create;
using ECommerce.APP.Payments;
using ECommerce.APP.Settings;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Infrastructure.Payments;

public sealed class StripePaymentService(
    IOptions<StripeSettings> stripeOptions,
    IRepository<Order> orderRepository,
    IUnitOfWork uow,
    ILogger<StripePaymentService> logger)
    : IPaymentService
{
    private readonly StripeSettings _settings = stripeOptions.Value;

    public async Task<ResultOfT<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit, 
        string currency, 
        Guid orderId, 
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        if (amountInSmallestUnit < 1)
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"payment_intent_create_{orderId}"
        };

        try
        {
            var paymentIntent = await new PaymentIntentService().CreateAsync(
                new PaymentIntentCreateOptions
                {
                    Amount = amountInSmallestUnit,
                    Currency = currency.ToLowerInvariant(),
                    PaymentMethodTypes = ["card"],
                    Metadata = new Dictionary<string, string>
                    {
                        ["OrderId"] = orderId.ToString()
                    }
                },
                requestOptions,
                ct
            );

            logger.LogInformation("Created PaymentIntent {Id} for Order {OrderId}", paymentIntent.Id, orderId);
            return ToResult(paymentIntent);
        }
        catch (StripeException ex) {
            logger.LogError(ex, "Stripe creation failed for Order {OrderId}, reason : {reason}", orderId, ex.Message);
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }
    }

    public async Task<ResultOfT<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId, 
        long amountInSmallestUnit, 
        Guid orderId, 
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        if (string.IsNullOrWhiteSpace(paymentIntentId))
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        if (amountInSmallestUnit < 1)
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"payment_intent_update_{orderId}_{paymentIntentId}_{amountInSmallestUnit}"
        };

        try
        {
            var paymentIntent = await new PaymentIntentService().UpdateAsync(
                paymentIntentId,
                new PaymentIntentUpdateOptions
                {
                    Amount = amountInSmallestUnit
                },
                requestOptions,
                ct
            );

            logger.LogInformation("Updated PaymentIntent {Id} for Order {OrderId}", paymentIntent.Id, orderId);
            return ToResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe update failed for Order {OrderId}, reason : {reason}", orderId, ex.Message);
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }
    }

    public async Task PaymentSucceeded(
        string paymentIntentId, 
        CancellationToken ct = default)
    {
        var order = await orderRepository.FirstOrDefaultAsync(
            new GetOrderByPaymentIntentSpecification(paymentIntentId),
            ct);

        if (order is null)
        {
            logger.LogWarning("PaymentSucceeded: no order for PaymentIntent {Id}", paymentIntentId);
            return;
        }

        var paid = order.MarkAsPaid(paymentIntentId);
        if (paid.IsFailure)
        {
            logger.LogWarning("PaymentSucceeded MarkAsPaid failed for {OrderId}: {Error}", order.Id, paid.Error!.Code);
            return;
        }

        orderRepository.Update(order);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Payment succeeded for Order {OrderId}, Intent {IntentId}", order.Id, paymentIntentId);
    }

    public async Task PaymentFailed(
        string paymentIntentId, 
        CancellationToken ct = default)
    {
        var order = await orderRepository.FirstOrDefaultAsync(
            new GetOrderByPaymentIntentSpecification(paymentIntentId),
            ct);

        if (order is null)
        {
            logger.LogWarning("PaymentFailed: no order for PaymentIntent {Id}", paymentIntentId);
            return;
        }

        var result = order.MarkAsPaymentFailed(paymentIntentId);
        if (result.IsFailure)
        {
            logger.LogInformation("PaymentFailed: no-op for Order {OrderId}, already in {Status}", order.Id, order.Status);
            return;
        }

        orderRepository.Update(order);
        await uow.SaveChangesAsync(ct);

        logger.LogWarning("Payment failed for Order {OrderId}, Intent {IntentId}", order.Id, paymentIntentId);
    }

    private ResultOfT<PaymentIntentResult> ToResult(PaymentIntent paymentIntent)
    {
        if (string.IsNullOrWhiteSpace(paymentIntent.ClientSecret))
        {
            logger.LogError("Payment Failed cuz ClientSecret does not exist");
            return ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }

        return ResultOfT<PaymentIntentResult>.Ok(new PaymentIntentResult(
            paymentIntent.Id,
            paymentIntent.ClientSecret,
            paymentIntent.Status,
            _settings.PublishableKey));
    }
}
