using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Common.Errors;

public static class CartErrors
{
    public static readonly Error GuestBuyerIdRequired =
        Error.Validation(
            "Cart.GuestBuyerIdRequired",
            "Guest shoppers must send the X-Buyer-Id header with a client-generated GUID.");

    public static readonly Error AuthenticatedBuyerIdMissing =
        Error.Validation(
            "Cart.AuthenticatedBuyerIdMissing",
            "The user id claim is missing or invalid in the authentication token.");

    public static readonly Error InvalidBuyerId =
        Error.Validation(
            "Cart.InvalidBuyerId",
            "A valid buyer id is required.");

    public static readonly Error InvalidProductId =
        Error.Validation(
            "Cart.InvalidProductId",
            "Product id is required.");

    public static readonly Error InvalidProductName =
        Error.Validation(
            "Cart.InvalidProductName",
            "Product name is required.");

    public static readonly Error InvalidPictureUrl =
        Error.Validation(
            "Cart.InvalidPictureUrl",
            "Product picture URL is required.");

    public static readonly Error InvalidUnitPrice =
        Error.Validation(
            "Cart.InvalidUnitPrice",
            "Unit price must be greater than zero.");

    public static readonly Error InvalidQuantity =
        Error.Validation(
            "Cart.InvalidQuantity",
            $"Quantity must be between {CartItem.MinQuantity} and {CartItem.MaxQuantity}.");

    public static readonly Error InvalidQuantityIncrement =
        Error.Validation(
            "Cart.InvalidQuantityIncrement",
            "Quantity increment must be greater than zero.");

    public static readonly Error QuantityTooHigh =
        Error.Validation(
            "Cart.QuantityTooHigh",
            $"Total quantity for a product cannot exceed {CartItem.MaxQuantity}.");

    public static readonly Error ItemNotFound =
        Error.NotFound(
            "Cart.ItemNotFound",
            "The product was not found in the Cart.");

    public static readonly Error AnonymousCartNotFound =
        Error.NotFound(
            "Cart.AnonymousCartNotFound",
            "The anonymous Cart to merge was not found or is already empty.");

    public static readonly Error CannotMergeSameCart =
        Error.Validation(
            "Cart.CannotMergeSameCart",
            "Cannot merge a cart into itself.");

    public static readonly Error AnonymousBuyerRequired =
        Error.Validation(
            "Cart.AnonymousBuyerRequired",
            "Anonymous buyer id is required for merge.");
}
