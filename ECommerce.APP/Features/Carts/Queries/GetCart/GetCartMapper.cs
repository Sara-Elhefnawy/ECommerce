using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Carts.Queries.GetCart;

public static class GetCartMapper
{
    public static GetCartResponse ToResponse(Cart cart)
        => new(
            BuyerId: cart.BuyerId,
            Items: cart.Items.Select(item => new CartItemResponse(
                item.ProductId,
                item.ProductName,
                item.PictureUrl,
                item.UnitPrice,
                item.Quantity,
                item.SubTotalPrice
            )).ToList(),
            TotalQuantity: cart.TotalQuantity,
            GrandTotalPrice: cart.GrandTotalPrice
        );
}
