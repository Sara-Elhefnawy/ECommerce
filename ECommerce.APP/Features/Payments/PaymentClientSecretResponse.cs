namespace ECommerce.APP.Features.Payments;

public sealed record PaymentClientSecretResponse(
    Guid OrderId,
    string PaymentIntentId,
    string ClientSecret,
    string Status,
    string PublishableKey);
