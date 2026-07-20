using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Carts.Commands.AddItemToCart.ProductLookup;

public sealed class ProductForCartSpecification : Specification<Product, ProductForCartResponse>
{
    public ProductForCartSpecification(Guid productId)
    {
        Query
            .Where(product => product.Id == productId)
            .Select(product => new ProductForCartResponse(
                product.Id, product.Name, product.PictureUrl, product.Price));
    }
}
