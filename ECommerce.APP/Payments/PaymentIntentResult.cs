namespace ECommerce.APP.Payments;

public sealed record PaymentIntentResult(
    string PaymentIntentId,
    string ClientSecret,
    string Status,                // payment status not order status
    string PublishableKey);
