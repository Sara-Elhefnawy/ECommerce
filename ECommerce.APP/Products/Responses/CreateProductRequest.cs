namespace ECommerce.APP.Products.Responses;

public record CreateProductRequest(
    string Name, 
    string Description, 
    string PictureUrl, 
    decimal Price, 
    Guid BrandId,
    Guid TypeId
    )
{
}
