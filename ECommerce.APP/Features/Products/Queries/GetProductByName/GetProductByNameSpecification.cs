using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Products.Queries.GetProductByName;

public sealed class GetProductByNameSpecification : Specification<Product>
{
    public GetProductByNameSpecification(string name)
    {
        Query
            .Where(p => p.Name.ToUpper().Trim().Equals(name.ToUpper().Trim()));
    }
}
