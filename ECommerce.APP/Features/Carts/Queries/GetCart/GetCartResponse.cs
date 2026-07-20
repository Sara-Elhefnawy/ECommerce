namespace ECommerce.APP.Features.Carts.Queries.GetCart;

public record GetCartResponse(
    Guid BuyerId,
    IReadOnlyList<CartItemResponse> Items,
    int TotalQuantity,
    decimal GrandTotalPrice);

public record CartItemResponse(
    Guid ProductId,
    string ProductName,
    string PictureUrl,
    decimal UnitPrice,
    int Quantity,
    decimal SubTotalPrice);
