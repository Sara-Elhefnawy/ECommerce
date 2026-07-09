namespace ECommerce.APP.Products.Commands;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Description,
    string PictureUrl,
    decimal Price,
    Guid BrandId,
    Guid TypeId
)
{ }
