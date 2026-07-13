using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Types.Queries.GetAll;

public sealed class GetAllTypesHandler(IReadRepository<ProductType> repository) :
    IRequestHandler<GetAllTypesQuery, ResultOfT<IReadOnlyList<GetAllTypesResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllTypesResponse>>> Handle(
        GetAllTypesQuery request, CancellationToken ct)
    {
        var types = await repository.ListAsync(new GetAllTypesSpecification(), ct);

        return ResultOfT<IReadOnlyList<GetAllTypesResponse>>.Ok(types);
    }
}
