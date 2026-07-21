using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities;

public sealed class Inventory : BaseEntity
{
    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = default!;

    public int QuantityOnHand { get; private set; }

    private Inventory() { }

    private Inventory(Guid productId, int quantity)
    {
        ProductId = productId;
        QuantityOnHand = quantity;
    }

    public static ResultOfT<Inventory> Create(
        Guid productId,
        int quantity)
    {
        if (productId == Guid.Empty)
            return ResultOfT<Inventory>.BadRequest(InventoryErrors.InvalidProduct);

        if (quantity <= 0)
            return ResultOfT<Inventory>.BadRequest(InventoryErrors.InvalidQuantity);

        return ResultOfT<Inventory>.Created(new Inventory(
            productId, quantity));
    }

    public bool HasEnough(int quantity) => QuantityOnHand >= quantity;

    public Result AddStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(InventoryErrors.InvalidQuantity);

        QuantityOnHand += quantity;

        return Result.Ok();
    }

    public Result RemoveStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(InventoryErrors.InvalidQuantity);

        if (QuantityOnHand < quantity)
            return Result.Failure(InventoryErrors.NotEnoughStock);

        QuantityOnHand -= quantity;

        return Result.Ok();
    }

    public Result SetStock(int quantity)
    {
        if (quantity < 0)
            return Result.Failure(InventoryErrors.InvalidQuantity);

        QuantityOnHand = quantity;

        return Result.Ok();
    }
}
