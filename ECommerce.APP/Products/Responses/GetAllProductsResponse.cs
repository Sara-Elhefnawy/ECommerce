namespace ECommerce.APP.Products.Responses;

public record GetAllProductsResponse(
    Guid Id,
    string Name,
    string Description,
    string PhotoUrl,
    decimal Price,
    string TypeName,
    string BrandName
)
{ }
