namespace ECommerce.APP.Features.Products.Queries.GetById;

public record GetProductByIdResponse(
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
