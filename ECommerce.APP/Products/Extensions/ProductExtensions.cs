using ECommerce.APP.Products.Commands;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Extensions;

public static class ProductExtensions
{
    public static CreateProductResponse ToCreateResponse(this Product product)
        => new
        (
            product.Id,
            product.Name,
            product.Description,
            product.PictureUrl,
            product.Price,
            product.BrandId,
            product.TypeId
        );
}
