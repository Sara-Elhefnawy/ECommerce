using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Brands.Queries.Details;

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
