namespace ECommerce.APP.Products.Responses.Extensions;

public static class ProductExtensions
{
    public static GetAllProductsResponse ToGetAllResponse(
        this Guid id,
        string name,
        string description,
        string photoUrl,
        decimal price,
        string typeName,
        string brandName)
        => new
        (
            id,
            name,
            description,
            photoUrl,
            price,
            typeName,
            brandName
        );


    public static DetailsProductResponse ToDetailsResponse(
        this Guid id,
        string name,
        string description,
        string photoUrl,
        decimal price,
        string typeName,
        string brandName)
        => new
        (
            id,
            name,
            description,
            photoUrl,
            price,
            typeName,
            brandName
        );
}
