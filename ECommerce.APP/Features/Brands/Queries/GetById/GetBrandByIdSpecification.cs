using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Brands.Queries.GetById;

public sealed class GetBrandByIdSpecification : Specification<ProductBrand, GetBrandByIdResponse>
{
    public GetBrandByIdSpecification(Guid id)
    {
        Query
            .Where(b => b.Id == id)
            .Select(b => new GetBrandByIdResponse(
                b.Id,
                b.Name));
    }
}
