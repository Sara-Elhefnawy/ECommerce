using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Types.Queries.GetById;

public sealed class GetTypeByIdSpecification : Specification<ProductType, GetTypeByIdResponse>
{
    public GetTypeByIdSpecification(Guid id)
    {
        Query
            .Where(b => b.Id == id)
            .Select(b => new GetTypeByIdResponse(
                b.Id,
                b.Name));
    }
}
