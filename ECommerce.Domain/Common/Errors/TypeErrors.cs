using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Common.Errors;

public static class TypeErrors
{
    // Validation Errors (400 Bad Request)
    public static readonly Error InvalidId = Error.Validation(
        "Type.InvalidId",
        "Type ID is required.");

    public static readonly Error InvalidName = Error.Validation(
        "Type.InvalidName",
        $"Type name cannot be empty or exceed {ProductType.MaxNameLength} characters.");

    public static readonly Error NameTooLong = Error.Validation(
        "Type.NameTooLong",
        $"Type name cannot exceed {ProductType.MaxNameLength} characters.");

    // Not Found Errors (404)
    public static readonly Error NotFound = Error.NotFound(
        "Type.NotFound",
        "The requested type was not found.");

    // Conflict Errors (409)
    public static readonly Error AlreadyExists = Error.Conflict(
        "Type.AlreadyExists",
        "A type with this name already exists.");

    // Failure Errors (500)
    public static readonly Error DatabaseError = Error.Failure(
        "Type.DatabaseError",
        "An error occurred while accessing the database.");
}
