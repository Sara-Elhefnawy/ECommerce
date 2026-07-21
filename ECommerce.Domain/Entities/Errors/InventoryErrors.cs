using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities.Errors;

public static class InventoryErrors
{
    public static readonly Error InvalidCount =
        Error.Validation(
            "Inventory.InvalidCount",
            "Invalid count requested.");

    public static readonly Error InvalidQuantity =
        Error.Validation(
            "Inventory.InvalidQuantity",
            "Quantity must be more than zero.");

    public static readonly Error InvalidProduct =
        Error.Validation(
            "Inventory.InvalidProduct",
            "Valid product id is required.");

    public static readonly Error NotEnoughStock =
        Error.Validation(
            "Inventory.NotEnoughStock",
            "Insufficient stock available.");

    public static readonly Error AlreadyExists =
        Error.Validation(
            "Inventory.AlreadyExists",
            "An inventory record already exists for this product.");

    // Not Found Errors (404)
    public static readonly Error NotFound = 
        Error.NotFound(
            "Inventory.NotFound",
            "The requested inventory was not found.");
}
