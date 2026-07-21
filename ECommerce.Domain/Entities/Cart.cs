using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using System.Text.Json.Serialization;

namespace ECommerce.Domain.Entities;

public sealed class Cart
{
    public Guid BuyerId { get; private set; }
    public List<CartItem> Items { get; private set; } = [];
    public int TotalQuantity => Items.Sum(item => item.Quantity);
    public decimal GrandTotalPrice => Items.Sum(item => item.SubTotalPrice);

    [JsonConstructor]
    private Cart(Guid buyerId, List<CartItem>? items)
    {
        BuyerId = buyerId;
        Items = items ?? [];
    }

    // when you first enter the cart without adding items it generate empty cart
    private Cart(Guid buyerId)
    {
        BuyerId = buyerId;
        Items = [];
    }

    public static ResultOfT<Cart> CreateEmpty(Guid buyerId)
    {
        if (buyerId == Guid.Empty)
            return ResultOfT<Cart>.BadRequest(CartErrors.InvalidBuyerId);

        return new Cart(buyerId);
    }

    // Adds a product or increases quantity when the product is already in the cart.
    public Result AddItem(
        Guid productId,
        string productName,
        string pictureUrl,
        decimal unitPrice,
        int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(CartErrors.InvalidQuantity);

        var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem is not null)
            return existingItem.IncreaseQuantity(quantity);

        var createResult = CartItem.Create(
            productId,
            productName,
            pictureUrl,
            unitPrice,
            quantity);

        if (createResult.IsFailure)
            return Result.Failure(createResult.Error!);

        Items.Add(createResult.Value);

        return Result.Ok();
    }

    public Result RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        Items.Remove(item);

        return Result.Ok();
    }

    public Result UpdateItemQuantity(Guid productId, int quantity)
    {
        if (quantity < 0)
            return Result.Failure(CartErrors.InvalidQuantity);

        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        if (quantity == 0)
            return RemoveItem(item.ProductId);

        return item.SetQuantity(quantity);
    }

    public void Clear() => Items.Clear();

    // Merges Cart (before login) into this one (after login)
    // Quantities for duplicate products are combined.
    public Result MergeCartFromGuestCart(Cart other)
    {
        // If it's the exact same cart instance, prevent merging
        if (ReferenceEquals(this, other))
            return Result.Failure(CartErrors.CannotMergeSameCart);

        foreach (var item in other.Items)
        {
            var mergeResult = AddItem(
                item.ProductId,
                item.ProductName,
                item.PictureUrl,
                item.UnitPrice,
                item.Quantity);

            if (mergeResult.IsFailure)
                return mergeResult;
        }

        return Result.Ok();
    }
}
