using ECommerce.APP.Types.Response;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Types.Queries;

public sealed class GetAllTypesQuery(ITypeQueryService queryService)
{
    public async Task<ResultOfT<IReadOnlyList<GetAllTypesResponse>>> Execute(CancellationToken ct = default)
    {
        var types = await queryService.GetAllAsync(ct);

        if (types is null || !types.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllTypesResponse>>.Ok(types);
    }
}
