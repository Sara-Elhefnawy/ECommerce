using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;
using System.Text.Json.Serialization;

namespace ECommerce.Domain.Entities;

public sealed class CartItem : BaseEntity
{
    public const int MinQuantity = 1;
    public const int MaxQuantity = 100;  // get quantity in DB

    // why not have only productId and quantity then get product data from somewhere else
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string PictureUrl { get; private set; } = default!;

    // set when item is added; updated when catalog price changes on cart read.
    // put limit to price so when update price is not set too high or too low
    public decimal UnitPrice { get; private set; }     
    
    public int Quantity { get; private set; }
    public decimal SubTotalPrice => UnitPrice * Quantity;

    // HybridCache saves carts as JSON, it uses System.Text.Json builtin library
    // so it needs public ctor to add the properties as JSON
    // [JsonConstructor] tells System.Text.Json to call this private constructor
    [JsonConstructor]
    private CartItem(
        Guid productId,
        string productName,
        string pictureUrl,
        decimal unitPrice,
        int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static ResultOfT<CartItem> Create(
        Guid productId,
        string productName,
        string pictureUrl,
        decimal unitPrice,
        int quantity)
    {
        if (productId == Guid.Empty)
            return ResultOfT<CartItem>.Failure(CartErrors.InvalidProductId);

        if (string.IsNullOrWhiteSpace(productName))
            return ResultOfT<CartItem>.Failure(CartErrors.InvalidProductName);

        if (string.IsNullOrWhiteSpace(pictureUrl))
            return ResultOfT<CartItem>.Failure(CartErrors.InvalidPictureUrl);

        if (unitPrice <= 0)
            return ResultOfT<CartItem>.Failure(CartErrors.InvalidUnitPrice);

        if (quantity is < MinQuantity or > MaxQuantity)
            return ResultOfT<CartItem>.Failure(CartErrors.InvalidQuantity);

        return new CartItem(
            productId,
            productName.Trim(),
            pictureUrl.Trim(),
            unitPrice,
            quantity);
    }

    public Result IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            return Result.Failure(CartErrors.InvalidQuantityIncrement);

        var newQuantity = Quantity + amount;

        if (newQuantity > MaxQuantity)
            return Result.Failure(CartErrors.QuantityTooHigh);

        Quantity = newQuantity;

        return Result.Ok();
    }

    public Result SetQuantity(int quantity)
    {
        if (quantity is < MinQuantity or > MaxQuantity)
            return Result.Failure(CartErrors.InvalidQuantity);

        Quantity = quantity;

        return Result.Ok();
    }

    // Updates unit price when catalog price has changed.
    // Unit price is copied from the catalog when the item is added.
    public Result UpdateUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            return Result.Failure(CartErrors.InvalidUnitPrice);

        UnitPrice = unitPrice;

        return Result.Ok();
    }
}
