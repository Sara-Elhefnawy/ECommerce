using ECommerce.APP.Brands.Response;
using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Brands.Specifications;

public sealed class DetailsBrandSpecification : Specification<ProductBrand, DetailsBrandResponse>
{
    public DetailsBrandSpecification(Guid id)
    {
        Query
            .Where(b => b.Id == id)
            .Select(b => new DetailsBrandResponse(
                b.Id,
                b.Name));
    }
}
