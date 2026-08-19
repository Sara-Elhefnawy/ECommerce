using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities;

public sealed class Order : BaseEntity
{
    public const int MaxDeliveryMethodNameLength = 100;
    public const int MaxDeliveryTimeLength = 100;

    private readonly List<OrderItem> _items = [];

    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }

    public Guid DeliveryMethodId { get; private set; }
    public string DeliveryMethodName { get; private set; } = default!;
    public decimal DeliveryMethodPrice { get; private set; }
    public string DeliveryMethodEstimatedTime { get; private set; } = default!;

    public ShippingAddress ShippingAddress { get; private set; } = default!;

    public decimal ItemsTotal { get; private set; }  // sum of all items' subtotal prices
    public decimal ShippingCost { get; private set; }
    public decimal Total { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items;

    private Order()
    {
    }

    public Order(
        Guid userId, 
        OrderStatus status, 
        Guid deliveryMethodId, 
        string deliveryMethodName, 
        decimal deliveryMethodPrice, 
        string deliveryMethodEstimatedTime, 
        ShippingAddress shippingAddress, 
        decimal shippingCost)
    {
        Id = Guid.NewGuid();

        UserId = userId;
        Status = status;
        DeliveryMethodId = deliveryMethodId;
        DeliveryMethodName = deliveryMethodName;
        DeliveryMethodPrice = deliveryMethodPrice;
        DeliveryMethodEstimatedTime = deliveryMethodEstimatedTime;
        ShippingAddress = shippingAddress;
        ShippingCost = shippingCost;
    }

    public static ResultOfT<Order> Create(
        Guid userId,
        DeliveryMethod deliveryMethod,
        UserAddress shippingAddress,
        IReadOnlyList<(Guid ProductId, string ProductName, string PictureUrl, decimal UnitPrice, int Quantity)> basketItems)
    {
        if (userId == Guid.Empty)
            return ResultOfT<Order>.Failure(OrderErrors.InvalidUserId);

        if (deliveryMethod is null)
            return ResultOfT<Order>.Failure(OrderErrors.DeliveryMethodRequired);

        if (!deliveryMethod.IsAvailable)
            return ResultOfT<Order>.Failure(OrderErrors.DeliveryMethodUnavailable);

        if (shippingAddress is null)
            return ResultOfT<Order>.Failure(OrderErrors.ShippingAddressRequired);

        if (shippingAddress.UserId != userId)
            return ResultOfT<Order>.Failure(OrderErrors.ShippingAddressNotOwned);

        if (basketItems is null || basketItems.Count == 0)
            return ResultOfT<Order>.Failure(OrderErrors.EmptyBasket);

        var addressSnapshot = ShippingAddress.FromUserAddress(shippingAddress);
        if (addressSnapshot.IsFailure)
            return ResultOfT<Order>.Failure(addressSnapshot.Error!);

        var order = new Order(
            userId,
            OrderStatus.Pending,
            deliveryMethod.Id,
            deliveryMethod.Name,
            deliveryMethod.Price,
            deliveryMethod.EstimatedDeliveryTime,
            addressSnapshot.Value,
            deliveryMethod.Price);

        foreach ((Guid ProductId, string ProductName, string PictureUrl, decimal UnitPrice, int Quantity) item in basketItems)
        {
            var snapshotResult = ProductItemOrdered.Create(
                item.ProductId,
                item.ProductName,
                item.PictureUrl,
                item.UnitPrice);

            if (snapshotResult.IsFailure)
                return ResultOfT<Order>.Failure(snapshotResult.Error!);

            var itemResult = OrderItem.Create(
                snapshotResult.Value,
                item.Quantity);

            if (itemResult.IsFailure)
                return ResultOfT<Order>.Failure(itemResult.Error!);

            itemResult.Value.AssignOrder(order.Id);
            order._items.Add(itemResult.Value);
        }

        order.ItemsTotal = order._items.Sum(i => i.SubTotalPrice);
        order.Total = order.ItemsTotal + order.ShippingCost;

        return ResultOfT<Order>.Created(order);
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.CannotCancel);

        Status = OrderStatus.Cancelled;

        return Result.Ok();
    }

    // AttachPaymentIntent / MarkAsPaid — see Stripe guide
}
