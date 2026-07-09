using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Brands.Queries.GetAll;

public sealed class GetAllBrandsSpecification : Specification<ProductBrand, GetAllBrandsResponse>
{
    public GetAllBrandsSpecification()
    {
        Query.OrderBy(brand => brand.Name)
            .Select(brand => new GetAllBrandsResponse(brand.Id, brand.Name));
    }
}
