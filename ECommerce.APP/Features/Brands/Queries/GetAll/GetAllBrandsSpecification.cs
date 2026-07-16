using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Brands.Queries.GetAll;

public sealed class GetAllBrandsSpecification : Specification<ProductBrand, GetAllBrandsResponse>
{
    public GetAllBrandsSpecification(int? count)
    {
        Query.OrderBy(b => b.Name);

        if (count is int value)
            Query.Take(value);

        Query.Select(b => new GetAllBrandsResponse(b.Id, b.Name, b.Products.Count));
    }
}
