namespace ECommerce.APP.Products.Responses;

public record DetailsProductResponse(
    Guid Id,
    string Name,
    string Description,
    string PictureUrl,
    decimal Price,
    string TypeName,
    string BrandName
    //int Quantity,
    //bool IsFavourite,
    //string Speciofication
)
{ }
