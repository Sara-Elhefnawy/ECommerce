using ECommerce.APP.Specifications;
using ECommerce.APP.Types.Response;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Types.Specifications;

public sealed class GetAllTypesSpecification : Specification<ProductType, GetAllTypesResponse>
{
    public GetAllTypesSpecification()
    {
        Query
            .OrderBy(type => type.Name)
            .Select(type => new GetAllTypesResponse(type.Id, type.Name));
    }
}
