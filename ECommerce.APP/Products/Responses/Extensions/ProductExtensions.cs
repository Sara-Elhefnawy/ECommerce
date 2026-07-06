using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Responses.Extensions;

public static class ProductExtensions
{
    public static GetAllProductsResponse ToGetAllResponse(
        this Guid id,
        string name,
        string description,
        string PictureUrl,
        decimal price,
        string typeName,
        string brandName)
        => new
        (
            id,
            name,
            description,
            PictureUrl,
            price,
            typeName,
            brandName
        );


    public static DetailsProductResponse ToDetailsResponse(
        this Guid id,
        string name,
        string description,
        string PictureUrl,
        decimal price,
        string typeName,
        string brandName)
        => new
        (
            id,
            name,
            description,
            PictureUrl,
            price,
            typeName,
            brandName
        );

    public static CreateProductRequest ToCreateResponse(this Product product)
        => new
        (
            product.Name,
            product.Description,
            product.PictureUrl,
            product.Price,
            product.BrandId,
            product.TypeId
        );
}
