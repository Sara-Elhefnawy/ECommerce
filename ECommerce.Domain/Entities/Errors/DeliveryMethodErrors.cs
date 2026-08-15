using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities.Errors;

public static class DeliveryMethodErrors
{
    public static readonly Error InvalidName =
        Error.Validation(
            "DeliveryMethod.InvalidName", 
            "Delivery method name is required.");

    public static readonly Error NameTooLong =
        Error.Validation(
            "DeliveryMethod.NameTooLong",
            $"Delivery method name cannot exceed {Entities.DeliveryMethod.MaxNameLength} characters.");

    public static readonly Error InvalidPrice =
        Error.Validation(
            "DeliveryMethod.InvalidPrice", 
            "Delivery price cannot be negative.");

    public static readonly Error InvalidDeliveryTime =
        Error.Validation(
            "DeliveryMethod.InvalidDeliveryTime", 
            "Estimated delivery time is required.");

    public static readonly Error NotFound =
        Error.NotFound(
            "DeliveryMethod.NotFound", 
            "Delivery method was not found.");

    public static readonly Error NameAlreadyExists =
        Error.Conflict(
            "DeliveryMethod.NameAlreadyExists", 
            "A delivery method with this name already exists.");

    public static readonly Error DescriptionTooLong =
        Error.Validation(
            "DeliveryMethod.Description.TooLong",
            $"Delivery method description cannot exceed {DeliveryMethod.MaxDescriptionLength} characters.");

    public static readonly Error DeliveryTimeTooLong =
        Error.Validation(
            "DeliveryMethod.EstimatedDeliveryTime.TooLong",
            $"Estimated delivery time cannot exceed {DeliveryMethod.MaxDeliveryTimeLength} characters.");

    public static readonly Error InvalidDisplayOrder =
        Error.Validation(
            "DeliveryMethod.InvalidDisplayOrder",
            "Display order must be greater than zero.");
}
