using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Orders.Mapper;

public static class OrderMapper
{
    public static OrderResponse ToResponse(Order order) =>
        new(
            order.Id,
            order.UserId,
            order.Status.ToString(),
            order.DeliveryMethodId,
            order.DeliveryMethodName,
            order.DeliveryMethodPrice,
            order.DeliveryMethodEstimatedTime,
            new ShippingAddressResponse(
                order.ShippingAddress.RecipientFirstName,
                order.ShippingAddress.RecipientLastName,
                order.ShippingAddress.PhoneNumber,
                order.ShippingAddress.Country,
                order.ShippingAddress.City,
                order.ShippingAddress.Street,
                order.ShippingAddress.PostalCode),
            order.ItemsTotal,
            order.ShippingCost,
            order.Total,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                new ProductItemOrderedResponse(
                    i.ItemOrdered.ProductId,
                    i.ItemOrdered.ProductName,
                    i.ItemOrdered.PictureUrl,
                    i.ItemOrdered.UnitPrice),
                i.Quantity,
                i.SubTotalPrice)).ToList());
}
