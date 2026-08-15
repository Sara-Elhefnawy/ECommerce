using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities;

// OrderItem is a child entity — it only exists inside an Order,
// is never loaded or queried on its own
public sealed class OrderItem
{
    // needs an Id for EF's tracking/equality purposes,
    // but audit fields like CreatedAt on an individual line item would be meaningless —
    // the order's creation time is what matters, not each item's
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public ProductItemOrdered ItemOrdered { get; private set; } = default!;
    public int Quantity { get; private set; }

    public decimal SubTotalPrice => ItemOrdered.UnitPrice * Quantity;

    internal void AssignOrder(Guid orderId) => OrderId = orderId;

    private OrderItem()
    {
    }

    public OrderItem(ProductItemOrdered itemOrdered, int quantity)
    {
        Id = Guid.NewGuid();

        ItemOrdered = itemOrdered;
        Quantity = quantity;
    }

    // Create method is internal specifically to enforce only Order to construct it
    internal static ResultOfT<OrderItem> Create(
        ProductItemOrdered itemOrdered,
        int quantity)
    {
        if (itemOrdered is null)
            return ResultOfT<OrderItem>.Failure(OrderErrors.InvalidProductId);

        if (quantity < 1)
            return ResultOfT<OrderItem>.Failure(OrderErrors.InvalidQuantity);

        return ResultOfT<OrderItem>.Ok(new OrderItem(
            itemOrdered,
            quantity
            ));
    }
}
