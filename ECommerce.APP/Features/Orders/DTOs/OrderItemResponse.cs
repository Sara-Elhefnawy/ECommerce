namespace ECommerce.APP.Features.Orders.DTOs;

public sealed record OrderItemResponse(
    Guid Id,
    ProductItemOrderedResponse ItemOrdered,
    int Quantity,
    decimal SubTotalPrice
    );
