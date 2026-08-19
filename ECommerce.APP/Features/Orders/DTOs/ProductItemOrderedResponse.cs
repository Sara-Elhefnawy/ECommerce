namespace ECommerce.APP.Features.Orders.DTOs;

public sealed record ProductItemOrderedResponse(
    Guid ProductId,
    string ProductName,
    string PictureUrl,
    decimal UnitPrice
    );