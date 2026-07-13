namespace ECommerce.APP.Features.Products.Commands;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Description,
    string PictureUrl,
    decimal Price,
    string BrandName,
    string TypeName
)
{ }
