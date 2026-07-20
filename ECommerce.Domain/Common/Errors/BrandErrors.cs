using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Common.Errors;

public static class BrandErrors
{
    // Validation Errors (400 Bad Request)
    public static readonly Error InvalidId = Error.Validation(
        "Brand.InvalidId",
        "Brand ID is required.");

    public static readonly Error InvalidName = Error.Validation(
        "Brand.InvalidName",
        $"Brand name cannot be empty or exceed {ProductBrand.MaxNameLength} characters.");

    public static readonly Error NameTooLong = Error.Validation(
        "Brand.NameTooLong",
        $"Brand name cannot exceed {ProductBrand.MaxNameLength} characters.");

    // Not Found Errors (404)
    public static readonly Error NotFound = Error.NotFound(
        "Brand.NotFound",
        "The requested brand was not found.");

    // Conflict Errors (409)
    public static readonly Error AlreadyExists = Error.Conflict(
        "Brand.AlreadyExists",
        "A brand with this name already exists.");

    // Failure Errors (500)
    public static readonly Error DatabaseError = Error.Failure(
        "Brand.DatabaseError",
        "An error occurred while accessing the database.");
}
