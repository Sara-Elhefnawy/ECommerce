using ECommerce.APP.Products.Responses;
using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Specifications;

public sealed class DetailsProductSpecification : Specification<Product, DetailsProductResponse>
{
    public DetailsProductSpecification(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            .Select(p => new DetailsProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.PictureUrl,
                p.Price,
                p.Type.Name,
                p.Brand.Name));
    }
}
