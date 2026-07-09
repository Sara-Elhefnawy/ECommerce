using ECommerce.APP.Types.Response;
using ECommerce.APP.Types.Specifications;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Types.Queries;

public sealed class GetAllTypesQuery(IReadRepository<ProductType> repository)
{
    public async Task<ResultOfT<IReadOnlyList<GetAllTypesResponse>>> Execute(CancellationToken ct = default)
    {
        var types = await repository.ListAsync(new GetAllTypesSpecification(), ct);

        if (types is null || !types.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllTypesResponse>>.Ok(types);
    }
}
