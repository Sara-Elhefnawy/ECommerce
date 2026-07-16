using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Types.Queries.GetAll;

public sealed class GetAllTypesSpecification : Specification<ProductType, GetAllTypesResponse>
{
    public GetAllTypesSpecification(int? count)
    {
        Query
            .OrderBy(type => type.Name);

        if (count is int value)
            Query.Take(count.Value);

        Query
            .Select(type => new GetAllTypesResponse(type.Id, type.Name, type.Products.Count));
    }
}
