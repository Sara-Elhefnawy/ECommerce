using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Types.Queries.GetByName;

public sealed class GetTypeByNameSpecification : Specification<ProductType, GetTypeByNameResponse>
{
    public GetTypeByNameSpecification(string name)
    {
        Query
            .Where(b => b.Name.Equals(name.ToUpperInvariant()))
            .Select(t => new GetTypeByNameResponse(t.Id, t.Name));
    }
}
