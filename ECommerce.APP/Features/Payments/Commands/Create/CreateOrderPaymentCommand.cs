using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Payments.Commands.Create;

public sealed record CreateOrderPaymentCommand(Guid OrderId) : IRequest<ResultOfT<PaymentClientSecretResponse>>;
