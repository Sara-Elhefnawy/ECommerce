namespace ECommerce.API.Endpoints;

public record CreateProductRequest(
    string Name,
    string Description,
    string PictureUrl,
    decimal Price,
    Guid BrandId,
    Guid TypeId);
