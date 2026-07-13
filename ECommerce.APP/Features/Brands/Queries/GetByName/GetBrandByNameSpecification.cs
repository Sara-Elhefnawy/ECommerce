using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Brands.Queries.GetByName;

public sealed class GetBrandByNameSpecification : Specification<ProductBrand, GetBrandByNameResponse>
{
    public GetBrandByNameSpecification(string name)
    {
        Query
            .Where(b => b.Name.Equals(name.ToUpperInvariant()))
            .Select(b => new GetBrandByNameResponse(b.Id, b.Name));
    }
}
