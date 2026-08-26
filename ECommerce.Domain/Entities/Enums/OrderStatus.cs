namespace ECommerce.Domain.Entities.Enums;

public enum OrderStatus
{
    Pending,
    PaymentFailed,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
