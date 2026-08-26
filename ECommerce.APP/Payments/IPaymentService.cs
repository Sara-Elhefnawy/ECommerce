using ECommerce.Domain.Results;

namespace ECommerce.APP.Payments;

public interface IPaymentService
{
    // must be long - always pay in dime so it convert to correct dollar value 
    //      if decimal then some of dollar value get lost
    Task<ResultOfT<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit,      
        string currency,
        Guid orderId,
        CancellationToken ct = default);

    Task<ResultOfT<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId,
        long amountInSmallestUnit,
        Guid orderId,
        CancellationToken ct = default);

    /// <summary>Called from webhook when payment_intent.succeeded.</summary>
    Task PaymentSucceeded(string paymentIntentId, CancellationToken ct = default);

    /// <summary>Called from webhook when payment_intent.payment_failed.</summary>
    Task PaymentFailed(string paymentIntentId, CancellationToken ct = default);
}
