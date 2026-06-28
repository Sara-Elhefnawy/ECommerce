using ECommerce.Domain.Common.Enums;

namespace ECommerce.Domain.Common;

// Result<Product>.Failure("Product.Already", "Product already exists");
public sealed record Error(string Code, string Message, ErrorTypes Type)
{
    public static Error Validation(string code, string message)
        => new(code, message, ErrorTypes.Validation);

    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorTypes.Forbidden);

    public static Error NotFound(string code, string message)
        => new(code, message, ErrorTypes.NotFound);

    public static Error UnAuthorized(string code, string message)
        => new(code, message, ErrorTypes.UnAuthorized);

    public static Error Conflict(string code, string message)
        => new(code, message, ErrorTypes.Conflict);

    public static Error Failure(string code, string message)
        => new(code, message, ErrorTypes.Failure);
}
