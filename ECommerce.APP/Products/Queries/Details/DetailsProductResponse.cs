namespace ECommerce.APP.Products.Queries.Details;

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
