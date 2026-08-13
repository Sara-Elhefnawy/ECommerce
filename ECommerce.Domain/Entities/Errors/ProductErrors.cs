using ECommerce.Domain.Images;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities.Errors;

public static class ProductErrors
{
    public static readonly Error InvalidCount = Error.Validation(
        "Product.InvalidCount",
        "Invalid count requested.");

    // Validation Errors (400 Bad Request)
    public static readonly Error InvalidName = Error.Validation(
        "Product.InvalidName",
        $"Product name cannot be empty or exceed {Product.MaxNameLength} characters.");

    public static readonly Error InvalidDescription = Error.Validation(
        "Product.InvalidDescription",
        $"Product description cannot be empty or exceed {Product.MaxDescriptionLength} characters.");

    public static readonly Error InvalidPrice = Error.Validation(
        "Product.InvalidPrice",
        "Product price cannot be negative.");

    public static readonly Error InvalidBrand = Error.Validation(
        "Product.InvalidBrand",
        "Valid product brand is required.");

    public static readonly Error InvalidType = Error.Validation(
        "Product.InvalidType",
        "Valid product type is required.");

    public static readonly Error AlreadyExists = Error.Conflict(
        "Product.AlreadyExists",
        "Product already exists with that name.");

    // Add new image validation errors
    public static readonly Error InvalidPictureUrl = Error.Validation(
        "Product.InvalidPictureUrl",
        $"Product picture URL cannot be empty or exceed {Product.MaxPictureUrlLength} characters.");

    public static readonly Error ImageRequired = Error.Validation(
        "Product.ImageRequired",
        "An image file is required");

    public static readonly Error ImageTooLarge = Error.Validation(
        "Product.ImageTooLarge",
        $"Image file size must be less than {ImageRules.MaxImageSizeInBytes / 1024 / 1024}MB");

    public static readonly Error InvalidImageExtension = Error.Validation(
        "Product.InvalidImageExtension",
        $"Image file must be one of: {string.Join(", ", ImageRules.AllowedExtensions)}");

    public static readonly Error InvalidImageType = Error.Validation(
        "Product.InvalidImageType",
        $"Image must be one of: {string.Join(", ", ImageRules.AllowedContentTypes)}");

    // Not Found Errors (404)
    public static readonly Error NotFound = Error.NotFound(
        "Product.NotFound",
        "The requested product was not found.");
}
